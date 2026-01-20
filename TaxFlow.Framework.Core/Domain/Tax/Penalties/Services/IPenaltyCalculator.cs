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

/// <summary>
/// Default implementation of IPenaltyRuleRegistry.
/// </summary>
public sealed class DefaultPenaltyRuleRegistry : IPenaltyRuleRegistry
{
    private readonly List<IPenaltyRule> _rules = [];

    public DefaultPenaltyRuleRegistry()
    {
        // Register default rules
        _rules.Add(new AssiettePenaltyRule());
        _rules.Add(new RecouvrementPenaltyRule());
    }

    public IEnumerable<IPenaltyRule> GetRules() => _rules.AsReadOnly();

    public void Register(IPenaltyRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        _rules.Add(rule);
    }

    public void Clear() => _rules.Clear();
}

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
