using Core.Domain.Tax.Payments;

namespace Core.Domain.Tax.Penalties.Services;

/// <summary>
/// Default implementation of IPenaltyCalculator.
/// </summary>
public sealed class DefaultPenaltyCalculator : IPenaltyCalculator
{
    private readonly IPenaltyRuleRegistry _ruleRegistry;

    /// <summary>
    /// Singleton instance with default registry.
    /// </summary>
    public static IPenaltyCalculator Default { get; } = new DefaultPenaltyCalculator(new DefaultPenaltyRuleRegistry());

    /// <summary>
    /// Constructor with rule registry injection.
    /// </summary>
    /// <param name="ruleRegistry"></param>
    public DefaultPenaltyCalculator(IPenaltyRuleRegistry ruleRegistry)
    {
        ArgumentNullException.ThrowIfNull(ruleRegistry);
        _ruleRegistry = ruleRegistry;
    }

    /// <summary>
    /// Calculate penalties as of a given date.
    /// </summary>
    public PenaltyCalculationResult Calculate(
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
    public PenaltyCalculationResult Calculate(
        PaymentSchedule schedule,
        PenaltyPolicy policy,
        DateTimeOffset asOf,
        decimal taxBaseAmount,
        PenaltyTriggerEvent triggerEvent,
        DateTimeOffset? assietteDueDate = null)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(policy);

        policy.Validate();

        var accruals = _ruleRegistry.GetRules()
            .SelectMany(r => r.Evaluate(schedule, policy, asOf, taxBaseAmount, assietteDueDate, triggerEvent))
            .ToList();

        return new PenaltyCalculationResult(accruals);
    }
}
