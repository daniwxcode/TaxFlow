using System;
using System.Collections.Generic;

namespace Core.Domain.Tax;

/// <summary>
/// Calculates assiette and recouvrement penalties with proration on unpaid amounts.
/// </summary>
public static class PenaltyCalculator
{
    /// <summary>
    /// Calculate penalties as of a given date.
    /// </summary>
    /// <param name="schedule">Payment schedule.</param>
    /// <param name="policy">Penalty policy.</param>
    /// <param name="asOf">Calculation date.</param>
    /// <param name="taxBaseAmount">Tax base amount for assiette penalty.</param>
    /// <param name="assietteDueDate">Assiette due date; if null, earliest installment due date is used.</param>
    public static PenaltyCalculationResult Calculate(
        PaymentSchedule schedule,
        PenaltyPolicy policy,
        DateTimeOffset asOf,
        decimal taxBaseAmount,
        DateTimeOffset? assietteDueDate = null)
    {
        if (schedule is null) throw new ArgumentNullException(nameof(schedule));
        if (policy is null) throw new ArgumentNullException(nameof(policy));

        policy.Validate();

        var accruals = new List<PenaltyAccrual>();
        var declarationId = schedule.DeclarationId == Guid.Empty ? Guid.NewGuid() : schedule.DeclarationId;
        var liquidationId = schedule.LiquidationId;

        var referenceDue = assietteDueDate ?? schedule.Installments.Select(i => i.DueDate).DefaultIfEmpty(asOf).Min();
        var assietteEffectiveDue = referenceDue.AddDays(policy.AssietteGraceDays);
        var assietteDaysLate = asOf > assietteEffectiveDue
            ? (int)Math.Max(0, (asOf.Date - assietteEffectiveDue.Date).TotalDays)
            : 0;

        // Fixed assiette penalty line
        if (policy.AssietteFixedAmount > 0)
        {
            accruals.Add(new PenaltyAccrual(
                PenaltyType.Assiette,
                PenaltyLineType.AssietteFixed,
                declarationId,
                liquidationId,
                null,
                taxBaseAmount,
                policy.AssietteFixedAmount,
                0m,
                assietteDaysLate,
                0,
                0,
                referenceDue,
                referenceDue,
                asOf));
        }

        // Assiette penalty on tax base (rate-based, periodic)
        if (policy.AssietteAnnualRate > 0 && taxBaseAmount > 0 && assietteDaysLate > 0)
        {
            var periodDays = policy.AssiettePeriodDays;
            var periods = (int)Math.Ceiling(assietteDaysLate / (double)periodDays);
            var baseAmount = taxBaseAmount;

            for (var p = 1; p <= periods; p++)
            {
                var periodStart = assietteEffectiveDue.AddDays((p - 1) * periodDays);
                var periodEnd = periodStart.AddDays(periodDays);
                var amount = Prorate(baseAmount, policy.AssietteAnnualRate, policy.DaysInYear, periodDays);
                amount = ApplyFloorAndCap(amount, policy.AssietteMinimum, policy.AssietteCap);

                accruals.Add(new PenaltyAccrual(
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
                    periodDays,
                    periodStart,
                    periodEnd,
                    asOf));

                if (policy.CapitalizeAssiette)
                    baseAmount += amount;
            }
        }

        // Recouvrement penalties per installment (prorata on late unpaid amount with periodicity)
        foreach (var inst in schedule.Installments)
        {
            var effectiveDue = inst.EffectiveDueDate.AddDays(policy.RecouvrementGraceDays);
            if (asOf <= effectiveDue) continue;

            var unpaid = inst.GetOutstanding(asOf);
            if (unpaid <= 0) continue;

            var daysLate = (int)Math.Max(0, (asOf.Date - effectiveDue.Date).TotalDays);
            if (daysLate == 0) continue;

            var periodDays = policy.RecouvrementPeriodDays;
            var periods = (int)Math.Ceiling(daysLate / (double)periodDays);
            var baseAmount = unpaid;

            for (var p = 1; p <= periods; p++)
            {
                var periodStart = effectiveDue.AddDays((p - 1) * periodDays);
                var periodEnd = periodStart.AddDays(periodDays);
                var penalty = Prorate(baseAmount, policy.RecouvrementAnnualRate, policy.DaysInYear, periodDays);
                penalty = ApplyFloorAndCap(penalty, policy.RecouvrementMinimum, policy.RecouvrementCap);

                accruals.Add(new PenaltyAccrual(
                    PenaltyType.Recouvrement,
                    PenaltyLineType.RecouvrementRate,
                    declarationId,
                    liquidationId,
                    inst.Id,
                    baseAmount,
                    penalty,
                    policy.RecouvrementAnnualRate,
                    daysLate,
                    p,
                    periodDays,
                    periodStart,
                    periodEnd,
                    asOf));

                if (policy.CapitalizeRecouvrement)
                    baseAmount += penalty;
            }
        }

        return new PenaltyCalculationResult(accruals);
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
