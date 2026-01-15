using System;

namespace Core.Domain.Tax;

/// <summary>
/// Options controlling tax engine evaluation.
/// </summary>
public sealed class TaxEngineOptions
{
    /// <summary>
    /// Evaluation date used for temporal filtering (UTC when null).
    /// </summary>
    public DateTimeOffset? ForDate { get; set; }

    /// <summary>
    /// Optional base amount exposed as the 'amount' variable in expressions.
    /// </summary>
    public decimal? BaseAmount { get; set; }

    /// <summary>
    /// Optional currency code to attach to lines and totals.
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Optional decimal precision for rounding.
    /// </summary>
    public int? Precision { get; set; }

    /// <summary>
    /// Optional rounding mode.
    /// </summary>
    public MidpointRounding? Rounding { get; set; }

    /// <summary>
    /// When true, attribute validation errors throw exceptions.
    /// </summary>
    public bool StrictValidation { get; set; } = true;

    /// <summary>
    /// When true, a rule evaluation error throws an exception.
    /// </summary>
    public bool ThrowOnRuleError { get; set; } = false;

    /// <summary>
    /// When true, include per-rule evaluation results in the result.
    /// </summary>
    public bool IncludeRuleResults { get; set; } = true;

    /// <summary>
    /// When true, include diagnostics for duplicate attributes.
    /// </summary>
    public bool DetectDuplicateAttributes { get; set; } = true;
}
