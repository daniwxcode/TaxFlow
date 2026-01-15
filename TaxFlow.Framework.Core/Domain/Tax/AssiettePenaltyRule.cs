using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Domain.Tax;

/// <summary>
/// Assiette penalty rule evaluator.
/// </summary>
public sealed class AssiettePenaltyRule : IPenaltyRule
{
    /// <inheritdoc />
    public IEnumerable<PenaltyAccrual> Evaluate(PaymentSchedule schedule, PenaltyPolicy policy, DateTimeOffset asOf, decimal taxBaseAmount, DateTimeOffset? assietteDueDate)
    {
        if (taxBaseAmount <= 0) yield break;

        var declarationId = schedule.DeclarationId == Guid.Empty ? Guid.NewGuid() : schedule.DeclarationId;
        var liquidationId = schedule.LiquidationId;

        var referenceDue = assietteDueDate ?? schedule.Installments.Select(i => i.DueDate).DefaultIfEmpty(asOf).Min();
        var assietteEffectiveDue = referenceDue.AddDays(policy.AssietteGraceDays);
        var assietteDaysLate = asOf > assietteEffectiveDue
            ? (int)Math.Max(0, (asOf.Date - assietteEffectiveDue.Date).TotalDays)
            : 0;

        if (policy.AssietteFixedAmount > 0 && assietteDaysLate > 0)
        {
            var fixedAmount = ApplyFloorAndCap(policy.AssietteFixedAmount, policy.AssietteMinimum, policy.AssietteCap);
            yield return new PenaltyAccrual(
                PenaltyType.Assiette,
                PenaltyLineType.AssietteFixed,
                declarationId,
                liquidationId,
                null,
                taxBaseAmount,
                fixedAmount,
                0m,
                assietteDaysLate,
                0,
                0,
                referenceDue,
                referenceDue,
                asOf);
        }

        if (policy.AssietteAnnualRate <= 0 || assietteDaysLate <= 0) yield break;

        var periodDays = policy.AssiettePeriodDays;
        var periods = (int)Math.Ceiling(assietteDaysLate / (double)periodDays);
        var baseAmount = taxBaseAmount;

        for (var p = 1; p <= periods; p++)
        {
            var periodStart = assietteEffectiveDue.AddDays((p - 1) * periodDays);
            var periodEnd = periodStart.AddDays(periodDays);
            var cappedEnd = periodEnd < asOf ? periodEnd : asOf;
            var daysInPeriod = (int)Math.Max(0, (cappedEnd.Date - periodStart.Date).TotalDays);
            if (daysInPeriod == 0) continue;

            var amount = Prorate(baseAmount, policy.AssietteAnnualRate, policy.DaysInYear, daysInPeriod);
            amount = ApplyFloorAndCap(amount, policy.AssietteMinimum, policy.AssietteCap);
            if (amount < policy.MinimumLineAmount) continue;

            yield return new PenaltyAccrual(
                PenaltyType.Assiette,
                PenaltyLineType.AssietteRate,
                declarationId,
                liquidationId,
                null,
                baseAmount,
                amount,
                policy.AssietteAnnualRate,
                assietteDaysLate,
                p,
                daysInPeriod,
                periodStart,
                cappedEnd,
                asOf);

            if (policy.CapitalizeAssiette)
                baseAmount += amount;
        }
    }

    private static decimal Prorate(decimal baseAmount, decimal annualRate, int daysInYear, int days)
    {
        if (baseAmount <= 0 || annualRate <= 0 || days <= 0) return 0m;
        return baseAmount * annualRate * days / daysInYear;
    }

    private static decimal ApplyFloorAndCap(decimal value, decimal? min, decimal? cap)
    {
        if (min.HasValue && value < min.Value) value = min.Value;
        if (cap.HasValue && value > cap.Value) value = cap.Value;
        return value;
    }
}
