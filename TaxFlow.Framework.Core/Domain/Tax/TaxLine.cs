using Core.Domain.Contracts.Abstracts;

namespace Core.Domain.Tax;

/// <summary>
/// Represents the result line of a tax calculation for a single rule.
/// </summary>
public class TaxLine: AuditableEntity
{
    public TaxLine(string key, string label, decimal amount)
    {
        Key = key ?? string.Empty;
        Label = label ?? string.Empty;
        Amount = amount;
    }

    public string Key { get; private set; }
    public string Label { get; private set; }
    public decimal Amount { get; private set; }
}
