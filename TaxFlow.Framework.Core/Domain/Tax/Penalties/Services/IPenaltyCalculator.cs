using Core.Domain.Tax.Payments;

namespace Core.Domain.Tax.Penalties.Services;

/// <summary>
/// Abstraction for penalty calculators.
/// Supports Dependency Inversion Principle and enables extensibility.
/// </summary>
public interface IPenaltyCalculator
{
    /// <summary>
    /// Calculate penalties as of a given date.
    /// </summary>
    PenaltyCalculationResult Calculate(
        PaymentSchedule schedule,
        PenaltyPolicy policy,
        DateTimeOffset asOf,
        decimal taxBaseAmount,
        DateTimeOffset? assietteDueDate = null);

    /// <summary>
    /// Calculate penalties as of a given date for a specific trigger event.
    /// </summary>
    PenaltyCalculationResult Calculate(
        PaymentSchedule schedule,
        PenaltyPolicy policy,
        DateTimeOffset asOf,
        decimal taxBaseAmount,
        PenaltyTriggerEvent triggerEvent,
        DateTimeOffset? assietteDueDate = null);
}

/// <summary>
/// Registry for penalty calculation rules.
/// Supports pluggable penalty rules via DI.
/// </summary>
public interface IPenaltyRuleRegistry
{
    /// <summary>
    /// Gets all registered penalty rules.
    /// </summary>
    IEnumerable<IPenaltyRule> GetRules();

    /// <summary>
    /// Registers a penalty rule.
    /// </summary>
    void Register(IPenaltyRule rule);

    /// <summary>
    /// Clears all registered rules.
    /// </summary>
    void Clear();
}
