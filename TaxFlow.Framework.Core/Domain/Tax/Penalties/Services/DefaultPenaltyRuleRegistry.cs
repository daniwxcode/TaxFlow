namespace Core.Domain.Tax.Penalties.Services;

/// <summary>
/// Default implementation of IPenaltyRuleRegistry.
/// </summary>
public sealed class DefaultPenaltyRuleRegistry : IPenaltyRuleRegistry
{
    private readonly List<IPenaltyRule> _rules = [];
    /// <summary>
    /// Constructor that initializes the registry with default penalty rules.
    /// </summary>
    public DefaultPenaltyRuleRegistry()
    {
        // Register default rules
        _rules.Add(new AssiettePenaltyRule());
        _rules.Add(new RecouvrementPenaltyRule());
    }

    /// <summary>
    /// Retrieves all registered penalty rules.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IPenaltyRule> GetRules() => _rules.AsReadOnly();

    /// <summary>
    /// Registers a penalty rule.
    /// </summary>
    /// <param name="rule"></param>
    public void Register(IPenaltyRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        _rules.Add(rule);
    }
    /// <summary>
    /// Clears all registered rules.
    /// </summary>
    public void Clear() => _rules.Clear();
}
