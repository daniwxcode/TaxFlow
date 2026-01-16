using System;
using System.Collections.Generic;

namespace Core.Domain.Tax;

/// <summary>
/// Recouvrement penalty rule evaluator.
/// </summary>
public sealed class RecouvrementPenaltyRule : IPenaltyRule
{
    /// <inheritdoc />
    public IEnumerable<PenaltyAccrual> Evaluate(PaymentSchedule schedule, PenaltyPolicy policy, DateTimeOffset asOf, decimal taxBaseAmount, DateTimeOffset? assietteDueDate, PenaltyTriggerEvent triggerEvent)
    {
        var definition = policy.GetDefinition(PenaltyType.Recouvrement);
        if (definition is null) yield break;
        if (definition.TriggerEvent != PenaltyTriggerEvent.Any && !definition.TriggerEvent.HasFlag(triggerEvent)) yield break;

        var declarationId = schedule.DeclarationId == Guid.Empty ? Guid.NewGuid() : schedule.DeclarationId;
        var liquidationId = schedule.LiquidationId;

        foreach (var inst in schedule.Installments)
        {
            var effectiveDue = inst.EffectiveDueDate.AddDays(definition.GraceDays);
            if (asOf <= effectiveDue) continue;

            var unpaid = inst.GetOutstanding(asOf);
            if (unpaid <= 0) continue;

            var daysLate = (int)Math.Max(0, (asOf.Date - effectiveDue.Date).TotalDays);
            if (daysLate == 0) continue;

            var periodDays = definition.PeriodDays;
            var periods = (int)Math.Ceiling(daysLate / (double)periodDays);
            var accumulatedPenalty = 0m;

            for (var p = 1; p <= periods; p++)
            {
                var periodStart = effectiveDue.AddDays((p - 1) * periodDays);
                var periodEnd = periodStart.AddDays(periodDays);
                var cappedEnd = periodEnd < asOf ? periodEnd : asOf;
                var daysInPeriod = (int)Math.Max(0, (cappedEnd.Date - periodStart.Date).TotalDays);
                if (daysInPeriod == 0) continue;

                var outstandingAtPeriodEnd = inst.GetOutstanding(cappedEnd);
                if (outstandingAtPeriodEnd <= 0) break;

                var baseAmount = definition.Capitalize
                    ? outstandingAtPeriodEnd + accumulatedPenalty
                    : outstandingAtPeriodEnd;
                var rate = definition.PeriodRate > 0
                    ? definition.PeriodRate + (p - 1) * definition.PeriodRateIncrement
                    : definition.AnnualRate;

                var penalty = definition.PeriodRate > 0
                    ? baseAmount * rate
                    : Prorate(baseAmount, rate, policy.DaysInYear, daysInPeriod);

                penalty = ApplyFloorAndCap(penalty, definition.Minimum, definition.Cap);
                if (penalty < policy.MinimumLineAmount) continue;

                yield return new PenaltyAccrual(
                    PenaltyType.Recouvrement,
                    PenaltyLineType.RecouvrementRate,
                    declarationId,
                    liquidationId,
                    inst.Id,
                    baseAmount,
                    penalty,
                    rate,
                    daysLate,
                    p,
                    daysInPeriod,
                    periodStart,
                    cappedEnd,
                    asOf);

                if (definition.Capitalize)
                    accumulatedPenalty += penalty;
            }
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
