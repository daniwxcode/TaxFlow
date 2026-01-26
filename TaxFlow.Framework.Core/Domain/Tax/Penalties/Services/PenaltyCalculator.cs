using Core.Domain.Tax.Payments;

namespace Core.Domain.Tax.Penalties.Services;

/// <summary>
/// Static facade for backward compatibility.
/// Use IPenaltyCalculator interface for new code.
/// </summary>
public static class PenaltyCalculator
{
    private static readonly IPenaltyCalculator Calculator = DefaultPenaltyCalculator.Default;

    /// <summary>
    /// Calculate penalties as of a given date.
    /// </summary>
    public static PenaltyCalculationResult Calculate(
        PaymentSchedule schedule,
        PenaltyPolicy policy,
        DateTimeOffset asOf,
        decimal taxBaseAmount,
        DateTimeOffset? assietteDueDate = null)
        => Calculator.Calculate(schedule, policy, asOf, taxBaseAmount, assietteDueDate);

    /// <summary>
    /// Calculate penalties as of a given date for a specific trigger event.
    /// </summary>
    public static PenaltyCalculationResult Calculate(
        PaymentSchedule schedule,
        PenaltyPolicy policy,
        DateTimeOffset asOf,
        decimal taxBaseAmount,
        PenaltyTriggerEvent triggerEvent,
        DateTimeOffset? assietteDueDate = null)
        => Calculator.Calculate(schedule, policy, asOf, taxBaseAmount, triggerEvent, assietteDueDate);
}
