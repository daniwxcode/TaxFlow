using Core.Domain.Contracts;
using Core.Domain.Contracts.Validation;
using Core.Domain.Localization;
using Core.Domain.Tax.Assets;

using System.Linq;

namespace Core.Domain.Tax.Calculation.Services;

/// <summary>
/// Abstraction for tax calculation engine.
/// Supports Dependency Inversion Principle and testability.
/// </summary>
public interface ITaxCalculationEngine
{
    /// <summary>
    /// Evaluate taxes for a single date.
    /// </summary>
    TaxCalculationResult Evaluate(TaxableAsset asset, TaxEngineOptions? options = null);

    /// <summary>
    /// Evaluate taxes for a period and apply simple prorata.
    /// </summary>
    TaxCalculationResult EvaluateForPeriod(
        TaxableAsset asset,
        DateTimeOffset from,
        DateTimeOffset to,
        int daysInYear = 365,
        TaxEngineOptions? options = null);
}

/// <summary>
/// High-performance tax engine for evaluating tax rules on a taxable asset.
/// </summary>
public sealed class TaxCalculationEngine : ITaxCalculationEngine
{
    /// <summary>
    /// Singleton instance with default implementation.
    /// </summary>
    public static ITaxCalculationEngine Default { get; } = new TaxCalculationEngine();

    /// <summary>
    /// Evaluate taxes for a single date.
    /// </summary>
    public TaxCalculationResult Evaluate(TaxableAsset asset, TaxEngineOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(asset);

        if (asset.AssetType is null)
        {
            throw new InvalidOperationException(ExceptionMessages.AssetTypeMustBeSetToEvaluate.Format());
        }

        options ??= new TaxEngineOptions();
        DateTimeOffset forDate = options.ForDate ?? DateTimeOffset.UtcNow;

        List<ExtendedAttribute> effectiveAttributes = GetEffectiveAttributes(asset, forDate);
        (List<string>? errors, List<string>? warnings) = ValidateAttributes(asset, effectiveAttributes, options);
        (List<TaxLine>? lines, List<TaxRuleEvaluationResult>? ruleResults) = EvaluateRules(asset, effectiveAttributes, forDate, options, errors);

        return BuildResult(lines, ruleResults, errors, warnings, options);
    }

    /// <summary>
    /// Evaluate taxes for a period and apply simple prorata.
    /// </summary>
    public TaxCalculationResult EvaluateForPeriod(
        TaxableAsset asset,
        DateTimeOffset from,
        DateTimeOffset to,
        int daysInYear = 365,
        TaxEngineOptions? options = null)
    {
        ValidatePeriodParameters(from, to, daysInYear);

        options ??= new TaxEngineOptions();
        TaxCalculationResult baseResult = Evaluate(asset, CreatePeriodOptions(options, from));
        decimal factor = CalculateProrataFactor(from, to, daysInYear);

        var proratedLines = baseResult.Lines
            .Select(l => new TaxLine(l.Key, l.Label, l.Amount * factor, options.Currency, options.Precision, options.Rounding))
            .ToList();

        return new TaxCalculationResult(
            proratedLines,
            baseResult.RuleResults,
            baseResult.Errors,
            baseResult.Warnings,
            options.Currency,
            options.Precision,
            options.Rounding);
    }

    #region Private Helpers

    private static List<ExtendedAttribute> GetEffectiveAttributes(TaxableAsset asset, DateTimeOffset forDate)
    {
        return asset.Attributes
            .Where(attr => attr.ValidFrom <= forDate && (attr.ValidTo == null || attr.ValidTo >= forDate))
            .ToList();
    }

    private static (List<string> errors, List<string> warnings) ValidateAttributes(
        TaxableAsset asset,
        List<ExtendedAttribute> effectiveAttributes,
        TaxEngineOptions options)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        if (options.DetectDuplicateAttributes)
        {
            DetectDuplicateAttributes(effectiveAttributes, warnings);
        }

        ValidationResult validationResult = asset.AssetType.ValidateAttributesResult(effectiveAttributes);
        if (validationResult.HasErrors)
        {
            if (options.StrictValidation)
            {
                throw new ArgumentException(ExceptionMessages.AttributeValidationFailed.Format(("errorMessage", validationResult.ErrorMessage)));
            }

            errors.AddRange(validationResult.ToMessages());
        }

        return (errors, warnings);
    }

    private static void DetectDuplicateAttributes(List<ExtendedAttribute> attributes, List<string> warnings)
    {
        var duplicateKeys = attributes
            .GroupBy(a => a.Key, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);
        warnings.AddRange(from key in duplicateKeys
                          select ExceptionMessages.DuplicateAttributeDetected.Format(("key", key)));
    }

    private static (List<TaxLine> lines, List<TaxRuleEvaluationResult>? ruleResults) EvaluateRules(
        TaxableAsset asset,
        List<ExtendedAttribute> effectiveAttributes,
        DateTimeOffset forDate,
        TaxEngineOptions options,
        List<string> errors)
    {
        List<TaxRule> rules = GetEffectiveRules(asset.AssetType, forDate);
        List<TaxLine> lines = new(rules.Count);
        List<TaxRuleEvaluationResult>? ruleResults = options.IncludeRuleResults ? new(rules.Count) : null;

        foreach (var rule in rules)
        {
            TaxRuleEvaluationResult result = asset.AssetType.EvaluateTaxRuleDetailed(
                rule.Key ?? string.Empty,
                effectiveAttributes,
                options.BaseAmount);

            HandleRuleResult(result, rule, options, errors);

            var amount = result.Value ?? 0m;
            lines.Add(new TaxLine(
                rule.Key ?? string.Empty,
                rule.Label ?? string.Empty,
                amount,
                options.Currency,
                options.Precision,
                options.Rounding));

            ruleResults?.Add(result);
        }

        return (lines, ruleResults);
    }

    private static List<TaxRule> GetEffectiveRules(AssetType assetType, DateTimeOffset forDate)
    {
        return assetType.TaxRules
            .Where(r => r.Enabled && r.ValidFrom <= forDate && (r.ValidTo == null || r.ValidTo >= forDate))
            .ToList();
    }

    private static void HandleRuleResult(
        TaxRuleEvaluationResult result,
        TaxRule rule,
        TaxEngineOptions options,
        List<string> errors)
    {
        if (result.IsSuccess)
        {
            return;
        }

        var message = result.ErrorMessage ?? ExceptionMessages.EvaluationFailed.Format();

        if (options.ThrowOnRuleError)
        {
            throw new InvalidOperationException(ExceptionMessages.RuleEvaluationFailed.Format(("ruleKey", rule.Key ?? ""), ("error", message)));
        }

        errors.Add(ExceptionMessages.RuleEvaluationFailed.Format(("ruleKey", rule.Key ?? ""), ("error", message)));
    }

    private static TaxCalculationResult BuildResult(
        List<TaxLine> lines,
        List<TaxRuleEvaluationResult>? ruleResults,
        List<string> errors,
        List<string> warnings,
        TaxEngineOptions options)
    {
        if (options.IncludeRuleResults)
        {
            return new TaxCalculationResult(
                lines,
                ruleResults,
                errors,
                warnings,
                options.Currency,
                options.Precision,
                options.Rounding);
        }

        return new TaxCalculationResult(lines, options.Currency, options.Precision, options.Rounding);
    }

    private static void ValidatePeriodParameters(DateTimeOffset from, DateTimeOffset to, int daysInYear)
    {
        if (to < from)
        {
            throw new ArgumentException(ExceptionMessages.EndDateMustBeGreaterOrEqual.Format(), nameof(to));
        }

        if (daysInYear <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(daysInYear), ExceptionMessages.DaysInYearMustBePositive.Format());
        }
    }

    private static TaxEngineOptions CreatePeriodOptions(TaxEngineOptions source, DateTimeOffset forDate)
    {
        return new TaxEngineOptions
        {
            ForDate = forDate,
            BaseAmount = source.BaseAmount,
            Currency = source.Currency,
            Precision = source.Precision,
            Rounding = source.Rounding,
            StrictValidation = source.StrictValidation,
            ThrowOnRuleError = source.ThrowOnRuleError,
            IncludeRuleResults = source.IncludeRuleResults,
            DetectDuplicateAttributes = source.DetectDuplicateAttributes
        };
    }

    private static decimal CalculateProrataFactor(DateTimeOffset from, DateTimeOffset to, int daysInYear)
    {
        var days = (to.Date - from.Date).TotalDays + 1;
        return (decimal)days / daysInYear;
    }

    #endregion
}

/// <summary>
/// Static facade for backward compatibility.
/// Use ITaxCalculationEngine interface for new code.
/// </summary>
public static class TaxEngine
{
    private static readonly ITaxCalculationEngine _engine = TaxCalculationEngine.Default;

    /// <summary>
    /// Evaluate taxes for a single date.
    /// </summary>
    public static TaxCalculationResult Evaluate(TaxableAsset asset, TaxEngineOptions? options = null)
        => _engine.Evaluate(asset, options);

    /// <summary>
    /// Evaluate taxes for a period and apply simple prorata.
    /// </summary>
    public static TaxCalculationResult EvaluateForPeriod(
        TaxableAsset asset,
        DateTimeOffset from,
        DateTimeOffset to,
        int daysInYear = 365,
        TaxEngineOptions? options = null)
        => _engine.EvaluateForPeriod(asset, from, to, daysInYear, options);
}
