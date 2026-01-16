using Core.Domain.Contracts.Abstracts;

using System;

namespace Core.Domain.Tax.Calculation;

/// <summary>
/// Represents the result line of a tax calculation for a single rule.
/// </summary>
public class TaxLine : AuditableEntity
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
        Currency = null;
        Precision = null;
        Rounding = null;
        RoundedAmount = null;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TaxLine"/> with currency and rounding metadata.
    /// </summary>
    /// <param name="key">The unique key of the tax rule that produced this line.</param>
    /// <param name="label">Human readable label for the tax line.</param>
    /// <param name="amount">Calculated monetary amount for this tax line.</param>
    /// <param name="currency">Optional currency code.</param>
    /// <param name="precision">Optional decimal precision for rounding.</param>
    /// <param name="rounding">Optional rounding mode.</param>
    public TaxLine(string key, string label, decimal amount, string? currency, int? precision, MidpointRounding? rounding)
    {
        Key = key ?? string.Empty;
        Label = label ?? string.Empty;
        Amount = amount;
        Currency = string.IsNullOrWhiteSpace(currency) ? null : currency.Trim();
        Precision = precision;
        Rounding = rounding;
        RoundedAmount = ComputeRoundedAmount(amount, precision, rounding);
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

    /// <summary>
    /// Optional currency code for this tax line.
    /// </summary>
    public string? Currency { get; private set; }

    /// <summary>
    /// Optional rounding precision used to compute <see cref="RoundedAmount"/>.
    /// </summary>
    public int? Precision { get; private set; }

    /// <summary>
    /// Optional rounding mode used to compute <see cref="RoundedAmount"/>.
    /// </summary>
    public MidpointRounding? Rounding { get; private set; }

    /// <summary>
    /// Optional rounded amount when precision is provided.
    /// </summary>
    public decimal? RoundedAmount { get; private set; }

    private static decimal? ComputeRoundedAmount(decimal amount, int? precision, MidpointRounding? rounding)
    {
        if (!precision.HasValue) return null;
        var mode = rounding ?? MidpointRounding.AwayFromZero;
        return Math.Round(amount, precision.Value, mode);
    }
}
