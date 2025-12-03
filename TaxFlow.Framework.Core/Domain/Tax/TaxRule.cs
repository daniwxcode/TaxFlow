using Core.Domain.Contracts;
using Core.Domain.Contracts.Abstracts;

namespace Core.Domain.Tax;

/// <summary>
/// Represents a tax rule attached to an asset type. The rule contains a dynamic expression
/// (NCalc) used to compute the taxable amount or tax due based on asset attributes.
/// </summary>
public class TaxRule : TemporalAuditableEntity
{
    /// <summary>
    /// Unique key for the rule.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Friendly label for the rule.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// The NCalc expression that computes the tax. It receives variables based on asset attributes.
    /// </summary>
    public string Expression { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the rule.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the rule is enabled and should be executed.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
