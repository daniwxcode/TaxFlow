using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Domain.Tax;

/// <summary>
/// Represents a detailed evaluation result for a tax rule.
/// </summary>
public class TaxRuleEvaluationResult
{
    private TaxRuleEvaluationResult(string ruleKey, decimal? value, bool isSuccess, string? errorMessage, IEnumerable<string> warnings)
    {
        RuleKey = ruleKey;
        Value = value;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Warnings = new ReadOnlyCollection<string>(new List<string>(warnings ?? new List<string>()));
    }

    /// <summary>
    /// Rule key used for evaluation.
    /// </summary>
    public string RuleKey { get; }

    /// <summary>
    /// Evaluated value when successful.
    /// </summary>
    public decimal? Value { get; }

    /// <summary>
    /// True if evaluation succeeded; otherwise false.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Error message when evaluation fails.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Warnings produced during evaluation (e.g., missing parameters).
    /// </summary>
    public IReadOnlyCollection<string> Warnings { get; }

    /// <summary>
    /// Create a successful evaluation result.
    /// </summary>
    public static TaxRuleEvaluationResult CreateSuccess(string ruleKey, decimal? value, IEnumerable<string>? warnings = null)
        => new TaxRuleEvaluationResult(ruleKey, value, true, null, warnings ?? new List<string>());

    /// <summary>
    /// Create a failed evaluation result.
    /// </summary>
    public static TaxRuleEvaluationResult CreateFailure(string ruleKey, string errorMessage)
        => new TaxRuleEvaluationResult(ruleKey, null, false, errorMessage, new List<string>());
}
