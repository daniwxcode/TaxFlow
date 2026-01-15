using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Core.Domain.Tax;

/// <summary>
/// Represents a tax calculation result with totals and optional rounding metadata.
/// </summary>
public class TaxCalculationResult
{
    /// <summary>
    /// Initializes a new instance of <see cref="TaxCalculationResult"/>.
    /// </summary>
    /// <param name="lines">Calculated tax lines.</param>
    /// <param name="currency">Optional currency code.</param>
    /// <param name="precision">Optional decimal precision for rounding.</param>
    /// <param name="rounding">Optional rounding mode.</param>
    public TaxCalculationResult(IEnumerable<TaxLine> lines, string? currency = null, int? precision = null, MidpointRounding? rounding = null)
    {
        var list = (lines ?? Array.Empty<TaxLine>()).ToList();
        Lines = new ReadOnlyCollection<TaxLine>(list);
        Currency = string.IsNullOrWhiteSpace(currency) ? null : currency.Trim();
        Precision = precision;
        Rounding = rounding;

        TotalAmount = list.Sum(l => l.Amount);

        if (precision.HasValue)
        {
            var mode = rounding ?? MidpointRounding.AwayFromZero;
            TotalRoundedAmount = Math.Round(TotalAmount, precision.Value, mode);
        }
        else if (list.All(l => l.RoundedAmount.HasValue))
        {
            TotalRoundedAmount = list.Sum(l => l.RoundedAmount!.Value);
        }
    }

    /// <summary>
    /// Read-only collection of tax lines.
    /// </summary>
    public IReadOnlyCollection<TaxLine> Lines { get; }

    /// <summary>
    /// Total amount before rounding.
    /// </summary>
    public decimal TotalAmount { get; }

    /// <summary>
    /// Total amount after rounding when applicable.
    /// </summary>
    public decimal? TotalRoundedAmount { get; }

    /// <summary>
    /// Optional currency code for the calculation result.
    /// </summary>
    public string? Currency { get; }

    /// <summary>
    /// Optional rounding precision for the calculation result.
    /// </summary>
    public int? Precision { get; }

    /// <summary>
    /// Optional rounding mode for the calculation result.
    /// </summary>
    public MidpointRounding? Rounding { get; }
}
