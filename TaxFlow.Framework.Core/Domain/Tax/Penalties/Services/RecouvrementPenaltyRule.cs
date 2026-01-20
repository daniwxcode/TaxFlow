using Core.Domain.Tax.Payments;

using System;
using System.Collections.Generic;

namespace Core.Domain.Tax.Penalties;

/// <summary>
/// Recouvrement penalty rule evaluator.
/// </summary>
public sealed class RecouvrementPenaltyRule : IPenaltyRule
{
    /// <inheritdoc />
    public IEnumerable<PenaltyAccrual> Evaluate(
        PaymentSchedule schedule,
        PenaltyPolicy policy,
        DateTimeOffset asOf,
        decimal taxBaseAmount,
        DateTimeOffset? assietteDueDate,
        PenaltyTriggerEvent triggerEvent)
    {
        var definition = policy.GetDefinition(PenaltyType.Recouvrement);
        if (definition is null)
            yield break;

        if (!PenaltyCalculationHelper.MatchesTriggerEvent(definition.TriggerEvent, triggerEvent))
            yield break;

        var declarationId = PenaltyCalculationHelper.GetOrCreateDeclarationId(schedule.DeclarationId);
        var liquidationId = schedule.LiquidationId;

        foreach (var inst in schedule.Installments)
        {
            var effectiveDue = definition.GracePeriod.AddTo(inst.EffectiveDueDate);
            if (asOf <= effectiveDue)
                continue;

            var unpaid = inst.GetOutstanding(asOf);
            if (unpaid <= 0)
                continue;

            var daysLate = PenaltyCalculationHelper.CalculateDaysLate(asOf, effectiveDue);
            if (daysLate == 0)
                continue;

            var periodDaysApprox = definition.Period.ToDays();
            var periodCount = PenaltyCalculationHelper.CalculatePeriodCount(daysLate, periodDaysApprox);
            var accumulatedPenalty = 0m;

            for (var period = 1; period <= periodCount; period++)
            {
                var periodStart = definition.GetPeriodStartDate(effectiveDue, period);
                var periodEnd = definition.GetPeriodEndDate(effectiveDue, period);
                var cappedEnd = periodEnd < asOf ? periodEnd : asOf;
                var daysInPeriod = (int)Math.Max(0, (cappedEnd.Date - periodStart.Date).TotalDays);

                if (daysInPeriod == 0)
                    continue;

                var outstandingAtPeriodEnd = inst.GetOutstanding(cappedEnd);
                if (outstandingAtPeriodEnd <= 0)
                    break;

                var baseAmount = definition.Capitalize
                    ? outstandingAtPeriodEnd + accumulatedPenalty
                    : outstandingAtPeriodEnd;

                var rate = definition.PeriodRate > 0
                    ? definition.PeriodRate + (period - 1) * definition.PeriodRateIncrement
                    : definition.AnnualRate;

                var penalty = definition.PeriodRate > 0
                    ? baseAmount * rate
                    : PenaltyCalculationHelper.Prorate(baseAmount, rate, policy.DaysInYear, daysInPeriod);

                penalty = PenaltyCalculationHelper.ApplyFloorAndCap(penalty, definition.Minimum, definition.Cap);

                if (penalty < policy.MinimumLineAmount)
                    continue;

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
                    period,
                    daysInPeriod,
                    periodStart,
                    cappedEnd,
                    asOf);

                if (definition.Capitalize)
                    accumulatedPenalty += penalty;
            }
        }
    }
}
