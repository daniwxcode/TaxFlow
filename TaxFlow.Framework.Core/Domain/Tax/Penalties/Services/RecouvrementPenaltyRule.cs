using Core.Domain.Tax.Payments;

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
        PenaltyDefinition? definition = policy.GetDefinition(PenaltyType.Recouvrement);
        if (definition is null)
        {
            yield break;
        }

        if (!PenaltyCalculationHelper.MatchesTriggerEvent(definition.TriggerEvent, triggerEvent))
        {
            yield break;
        }

        Guid declarationId = PenaltyCalculationHelper.GetOrCreateDeclarationId(schedule.DeclarationId);
        Guid? liquidationId = schedule.LiquidationId;

        foreach (var inst in schedule.Installments)
        {
            DateTimeOffset effectiveDue = definition.GracePeriod.AddTo(inst.EffectiveDueDate);
            if (asOf <= effectiveDue)
            {
                continue;
            }

            decimal unpaid = inst.GetOutstanding(asOf);
            if (unpaid <= 0)
            {
                continue;
            }

            int daysLate = PenaltyCalculationHelper.CalculateDaysLate(asOf, effectiveDue);
            if (daysLate == 0)
            {
                continue;
            }

            int periodDaysApprox = definition.Period.ToDays();
            int periodCount = PenaltyCalculationHelper.CalculatePeriodCount(daysLate, periodDaysApprox);
            decimal accumulatedPenalty = 0m;

            for (int period = 1; period <= periodCount; period++)
            {
                DateTimeOffset periodStart = definition.GetPeriodStartDate(effectiveDue, period);
                DateTimeOffset periodEnd = definition.GetPeriodEndDate(effectiveDue, period);
                DateTimeOffset cappedEnd = periodEnd < asOf ? periodEnd : asOf;
                int daysInPeriod = (int)Math.Max(0, (cappedEnd.Date - periodStart.Date).TotalDays);

                if (daysInPeriod == 0)
                {
                    continue;
                }

                decimal outstandingAtPeriodEnd = inst.GetOutstanding(cappedEnd);
                if (outstandingAtPeriodEnd <= 0)
                {
                    break;
                }

                decimal baseAmount = definition.Capitalize
                    ? outstandingAtPeriodEnd + accumulatedPenalty
                    : outstandingAtPeriodEnd;

                decimal rate = definition.PeriodRate > 0
                    ? definition.PeriodRate + (period - 1) * definition.PeriodRateIncrement
                    : definition.AnnualRate;

                decimal penalty = definition.PeriodRate > 0
                    ? baseAmount * rate
                    : PenaltyCalculationHelper.Prorate(baseAmount, rate, policy.DaysInYear, daysInPeriod);

                penalty = PenaltyCalculationHelper.ApplyFloorAndCap(penalty, definition.Minimum, definition.Cap);

                if (penalty < policy.MinimumLineAmount)
                {
                    continue;
                }

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
                {
                    accumulatedPenalty += penalty;
                }
            }
        }
    }
}
