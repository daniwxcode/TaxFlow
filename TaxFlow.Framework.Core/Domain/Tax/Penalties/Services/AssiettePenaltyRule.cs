using Core.Domain.Tax.Payments;

namespace Core.Domain.Tax.Penalties;

/// <summary>
/// Assiette penalty rule evaluator.
/// </summary>
public sealed class AssiettePenaltyRule : IPenaltyRule
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
        if (taxBaseAmount <= 0)
        {
            yield break;
        }

        PenaltyDefinition? definition = policy.GetDefinition(PenaltyType.Assiette);
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

        DateTimeOffset referenceDue = assietteDueDate ?? schedule.Installments.Select(i => i.DueDate).DefaultIfEmpty(asOf).Min();
        DateTimeOffset effectiveDue = definition.GracePeriod.AddTo(referenceDue);
        int daysLate = PenaltyCalculationHelper.CalculateDaysLate(asOf, effectiveDue);

        if (daysLate <= 0)
        {
            yield break;
        }

        // Fixed penalty
        if (definition.FixedAmount > 0)
        {
            var fixedAmount = PenaltyCalculationHelper.ApplyFloorAndCap(
                definition.FixedAmount,
                definition.Minimum,
                definition.Cap);

            yield return new PenaltyAccrual(
                PenaltyType.Assiette,
                PenaltyLineType.AssietteFixed,
                declarationId,
                liquidationId,
                installmentId: null,
                taxBaseAmount,
                fixedAmount,
                rate: 0m,
                daysLate,
                periodIndex: 0,
                periodDays: 0,
                referenceDue,
                referenceDue,
                asOf);
        }

        // Periodic penalties
        if (definition.AnnualRate <= 0)
        {
            yield break;
        }

        int periodDaysApprox = definition.Period.ToDays();
        int periodCount = PenaltyCalculationHelper.CalculatePeriodCount(daysLate, periodDaysApprox);
        decimal baseAmount = taxBaseAmount;

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

            decimal amount = PenaltyCalculationHelper.Prorate(baseAmount, definition.AnnualRate, policy.DaysInYear, daysInPeriod);
            amount = PenaltyCalculationHelper.ApplyFloorAndCap(amount, definition.Minimum, definition.Cap);

            if (amount < policy.MinimumLineAmount)
            {
                continue;
            }

            yield return new PenaltyAccrual(
                PenaltyType.Assiette,
                PenaltyLineType.AssietteRate,
                declarationId,
                liquidationId,
                installmentId: null,
                baseAmount,
                amount,
                definition.AnnualRate,
                daysLate,
                period,
                daysInPeriod,
                periodStart,
                cappedEnd,
                asOf);

            if (definition.Capitalize)
            {
                baseAmount += amount;
            }
        }
    }
}
