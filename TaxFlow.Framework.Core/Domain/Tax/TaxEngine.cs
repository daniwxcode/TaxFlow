using Core.Domain.Contracts;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Core.Domain.Tax;

/// <summary>
/// High-performance tax engine for evaluating tax rules on a taxable asset.
/// </summary>
public static class TaxEngine
{
    /// <summary>
    /// Evaluate taxes for a single date.
    /// </summary>
    /// <param name="asset">Asset to evaluate.</param>
    /// <param name="options">Optional engine options.</param>
    /// <returns>Tax calculation result with diagnostics.</returns>
    public static TaxCalculationResult Evaluate(TaxableAsset asset, TaxEngineOptions? options = null)
    {
        if (asset is null) throw new ArgumentNullException(nameof(asset));
        if (asset.AssetType is null) throw new InvalidOperationException("AssetType must be set to evaluate taxes.");

        options ??= new TaxEngineOptions();
        var forDate = options.ForDate ?? DateTimeOffset.UtcNow;

        var effectiveAttributes = new List<ExtendedAttribute>();
        foreach (var attr in asset.Attributes)
        {
            if (attr.ValidFrom <= forDate && (attr.ValidTo == null || attr.ValidTo >= forDate))
                effectiveAttributes.Add(attr);
        }

        var errors = new List<string>();
        var warnings = new List<string>();

        if (options.DetectDuplicateAttributes)
        {
            var duplicateKeys = effectiveAttributes
                .GroupBy(a => a.Key, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            foreach (var key in duplicateKeys)
                warnings.Add($"Attribut dupliqué détecté pour la clé '{key}'.");
        }

        var validationErrors = asset.AssetType.ValidateAttributes(effectiveAttributes).ToList();
        if (validationErrors.Any())
        {
            if (options.StrictValidation)
                throw new ArgumentException($"Attributes validation failed: {string.Join("; ", validationErrors)}");
            errors.AddRange(validationErrors);
        }

        var rules = asset.AssetType.TaxRules
            .Where(r => r.Enabled && r.ValidFrom <= forDate && (r.ValidTo == null || r.ValidTo >= forDate))
            .ToList();

        var lines = new List<TaxLine>(rules.Count);
        var ruleResults = options.IncludeRuleResults ? new List<TaxRuleEvaluationResult>(rules.Count) : null;

        foreach (var rule in rules)
        {
            var result = asset.AssetType.EvaluateTaxRuleDetailed(rule.Key ?? string.Empty, effectiveAttributes, options.BaseAmount);

            if (!result.IsSuccess)
            {
                var message = result.ErrorMessage ?? "Rule evaluation failed.";
                if (options.ThrowOnRuleError)
                    throw new InvalidOperationException($"Rule '{rule.Key}': {message}");
                errors.Add($"Rule '{rule.Key}': {message}");
            }

            var amount = result.Value ?? 0m;
            lines.Add(new TaxLine(rule.Key ?? string.Empty, rule.Label ?? string.Empty, amount, options.Currency, options.Precision, options.Rounding));

            if (ruleResults != null)
                ruleResults.Add(result);
        }

        if (options.IncludeRuleResults)
        {
            return new TaxCalculationResult(lines, ruleResults, errors, warnings, options.Currency, options.Precision, options.Rounding);
        }

        return new TaxCalculationResult(lines, options.Currency, options.Precision, options.Rounding);
    }

    /// <summary>
    /// Evaluate taxes for a period and apply simple prorata based on number of days.
    /// </summary>
    /// <param name="asset">Asset to evaluate.</param>
    /// <param name="from">Period start (inclusive).</param>
    /// <param name="to">Period end (inclusive).</param>
    /// <param name="daysInYear">Proration basis (default 365).</param>
    /// <param name="options">Optional engine options.</param>
    /// <returns>Tax calculation result with diagnostics.</returns>
    public static TaxCalculationResult EvaluateForPeriod(TaxableAsset asset, DateTimeOffset from, DateTimeOffset to, int daysInYear = 365, TaxEngineOptions? options = null)
    {
        if (to < from) throw new ArgumentException("The end date must be greater than or equal to the start date.", nameof(to));
        if (daysInYear <= 0) throw new ArgumentOutOfRangeException(nameof(daysInYear), "daysInYear must be greater than 0.");

        options ??= new TaxEngineOptions();

        var baseResult = Evaluate(asset, new TaxEngineOptions
        {
            ForDate = from,
            BaseAmount = options.BaseAmount,
            Currency = options.Currency,
            Precision = options.Precision,
            Rounding = options.Rounding,
            StrictValidation = options.StrictValidation,
            ThrowOnRuleError = options.ThrowOnRuleError,
            IncludeRuleResults = options.IncludeRuleResults,
            DetectDuplicateAttributes = options.DetectDuplicateAttributes
        });

        var days = (to.Date - from.Date).TotalDays + 1;
        var factor = (decimal)days / daysInYear;

        var proratedLines = baseResult.Lines
            .Select(l => new TaxLine(l.Key, l.Label, l.Amount * factor, options.Currency, options.Precision, options.Rounding))
            .ToList();

        var ruleResults = baseResult.RuleResults;
        var errors = baseResult.Errors;
        var warnings = baseResult.Warnings;

        return new TaxCalculationResult(proratedLines, ruleResults, errors, warnings, options.Currency, options.Precision, options.Rounding);
    }
}
