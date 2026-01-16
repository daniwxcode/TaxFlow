using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Domain.Tax;

/// <summary>
/// Calculates assiette and recouvrement penalties with proration on unpaid amounts.
/// </summary>
public static class PenaltyCalculator
{
    private static readonly IReadOnlyList<IPenaltyRule> Rules = new IPenaltyRule[]
    {
        new AssiettePenaltyRule(),
        new RecouvrementPenaltyRule()
    };

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
        return Calculate(schedule, policy, asOf, taxBaseAmount, PenaltyTriggerEvent.Any, assietteDueDate);
    }

    /// <summary>
    /// Calculate penalties as of a given date for a specific trigger event.
    /// </summary>
    /// <param name="schedule">Payment schedule.</param>
    /// <param name="policy">Penalty policy.</param>
    /// <param name="asOf">Calculation date.</param>
    /// <param name="taxBaseAmount">Tax base amount for assiette penalty.</param>
    /// <param name="triggerEvent">Trigger event used to filter penalties.</param>
    /// <param name="assietteDueDate">Assiette due date; if null, earliest installment due date is used.</param>
    public static PenaltyCalculationResult Calculate(
        PaymentSchedule schedule,
        PenaltyPolicy policy,
        DateTimeOffset asOf,
        decimal taxBaseAmount,
        PenaltyTriggerEvent triggerEvent,
        DateTimeOffset? assietteDueDate = null)
    {
        if (schedule is null) throw new ArgumentNullException(nameof(schedule));
        if (policy is null) throw new ArgumentNullException(nameof(policy));

        policy.Validate();

        var accruals = Rules
            .SelectMany(r => r.Evaluate(schedule, policy, asOf, taxBaseAmount, assietteDueDate, triggerEvent))
            .ToList();

        return new PenaltyCalculationResult(accruals);
    }
}
