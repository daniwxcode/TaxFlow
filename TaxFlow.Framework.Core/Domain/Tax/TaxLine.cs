using Core.Domain.Contracts.Abstracts;

namespace Core.Domain.Tax;

/// <summary>
/// Represents the result line of a tax calculation for a single rule.
/// </summary>
public class TaxLine: AuditableEntity
{
    /// <summary>
    /// Initializes a new instance of <see cref="TaxLine"/>.
    /// </summary>
    /// <param name="key">The unique key of the tax rule that produced this line.</param>
    /// <param name="label">Human readable label for the tax line.</param>
    /// <param name="amount">Calculated monetary amount for this tax line.</param>
    public TaxLine(string key, string label, decimal amount)
    {
        Key = key ?? string.Empty;
        Label = label ?? string.Empty;
        Amount = amount;
    }

    /// <summary>
    /// Gets the tax rule key that produced this line.
    /// </summary>
    public string Key { get; private set; }

    /// <summary>
    /// Gets the human readable label describing the tax line.
    /// </summary>
    public string Label { get; private set; }

    /// <summary>
    /// Gets the calculated amount for this tax line.
    /// </summary>
    public decimal Amount { get; private set; }
}
