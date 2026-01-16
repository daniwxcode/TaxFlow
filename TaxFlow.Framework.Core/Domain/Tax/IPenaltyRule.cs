using System;
using System.Collections.Generic;

namespace Core.Domain.Tax;

/// <summary>
/// Defines a penalty rule evaluator.
/// </summary>
public interface IPenaltyRule
{
    /// <summary>
    /// Evaluate penalties for the given schedule and policy.
    /// </summary>
    /// <param name="schedule">Payment schedule.</param>
    /// <param name="policy">Penalty policy.</param>
    /// <param name="asOf">Calculation date.</param>
    /// <param name="taxBaseAmount">Tax base amount for assiette penalties.</param>
    /// <param name="assietteDueDate">Assiette due date (optional).</param>
    /// <returns>Penalty accrual lines.</returns>
    IEnumerable<PenaltyAccrual> Evaluate(PaymentSchedule schedule, PenaltyPolicy policy, DateTimeOffset asOf, decimal taxBaseAmount, DateTimeOffset? assietteDueDate, PenaltyTriggerEvent triggerEvent);
}
