using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Localization;
using Core.Domain.Tax.Assets;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Core.Domain.Tax.Calculation;

/// <summary>
/// Provides orchestration utilities to liquidate multiple taxable assets in a single pass.
/// </summary>
public static class TaxLiquidationPipeline
{
    /// <summary>
    /// Evaluates a collection of taxable assets, applying grouped liquidation when required.
    /// </summary>
    public static TaxCalculationResult Evaluate(IEnumerable<TaxableAsset> assets, TaxEngineOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(assets);

        options ??= new TaxEngineOptions();
        var assetList = assets.Where(a => a is not null).ToList();

        if (assetList.Count == 0)
            return options.IncludeRuleResults
                ? new TaxCalculationResult(Array.Empty<TaxLine>(), Array.Empty<TaxRuleEvaluationResult>(), Array.Empty<string>(), Array.Empty<string>(), options.Currency, options.Precision, options.Rounding)
                : new TaxCalculationResult(Array.Empty<TaxLine>(), options.Currency, options.Precision, options.Rounding);

        var evaluationDate = options.ForDate ?? DateTimeOffset.UtcNow;
        var lines = new List<TaxLine>();
        var errors = new List<string>();
        var warnings = new List<string>();
        var ruleResults = options.IncludeRuleResults ? new List<TaxRuleEvaluationResult>() : null;

        foreach (var group in assetList.GroupBy(a => a.AssetTypeId))
        {
            var assetType = group.First().AssetType;
            if (assetType is null)
                throw new InvalidOperationException(ExceptionMessages.AssetTypeMustBeSetToEvaluate.Format());

            if (assetType.LiquidationMode == LiquidationMode.Grouped)
            {
                var aggregatedAsset = AggregateAssets(assetType, group, evaluationDate);
                AppendResult(TaxEngine.Evaluate(aggregatedAsset, options), lines, ruleResults, errors, warnings);
            }
            else
            {
                foreach (var asset in group)
                    AppendResult(TaxEngine.Evaluate(asset, options), lines, ruleResults, errors, warnings);
            }
        }

        if (options.IncludeRuleResults)
        {
            var resultRules = ruleResults ?? new List<TaxRuleEvaluationResult>();
            return new TaxCalculationResult(
                lines,
                resultRules,
                errors,
                warnings,
                options.Currency,
                options.Precision,
                options.Rounding);
        }

        return new TaxCalculationResult(lines, options.Currency, options.Precision, options.Rounding);
    }

    private static TaxableAsset AggregateAssets(AssetType assetType, IEnumerable<TaxableAsset> assets, DateTimeOffset forDate)
    {
        var aggregate = new Dictionary<string, AggregatedAttributeState>(StringComparer.OrdinalIgnoreCase);

        foreach (var attribute in assets.SelectMany(asset => asset.Attributes
                     .Where(attr => attr.ValidFrom <= forDate && (attr.ValidTo == null || attr.ValidTo >= forDate))))
        {
            if (!aggregate.TryGetValue(attribute.Key, out var state))
            {
                state = new AggregatedAttributeState(attribute);
                aggregate[attribute.Key] = state;
            }

            state.Add(attribute);
        }

        var aggregatedAttributes = new Collection<ExtendedAttribute>(
            aggregate.Values.Select(state => state.ToAttribute(forDate)).ToList());

        return TaxableAsset.Create(assetType, aggregatedAttributes);
    }

    private static void AppendResult(
        TaxCalculationResult result,
        List<TaxLine> lines,
        List<TaxRuleEvaluationResult>? ruleResults,
        List<string> errors,
        List<string> warnings)
    {
        lines.AddRange(result.Lines);
        if (ruleResults != null)
            ruleResults.AddRange(result.RuleResults);
        errors.AddRange(result.Errors);
        warnings.AddRange(result.Warnings);
    }

    private sealed class AggregatedAttributeState
    {
        private readonly string _key;
        private readonly AttributeDataType _dataType;
        private bool _isRequired;
        private decimal _numericTotal;
        private string? _value;
        private bool _hasValue;

        public AggregatedAttributeState(ExtendedAttribute template)
        {
            _key = template.Key;
            _dataType = template.DataType;
        }

        public void Add(ExtendedAttribute attribute)
        {
            _isRequired |= attribute.IsRequired;

            if (_dataType == AttributeDataType.Number)
            {
                _numericTotal += ParseDecimal(attribute.Value);
            }
            else if (!_hasValue && !string.IsNullOrWhiteSpace(attribute.Value))
            {
                _value = attribute.Value;
                _hasValue = true;
            }
        }

        public ExtendedAttribute ToAttribute(DateTimeOffset forDate)
        {
            var storedValue = _dataType == AttributeDataType.Number
                ? _numericTotal.ToString(CultureInfo.InvariantCulture)
                : _value ?? string.Empty;

            return ExtendedAttribute.Create(_key, storedValue, _dataType, _isRequired, forDate);
        }

        private static decimal ParseDecimal(string value)
        {
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed))
                return parsed;

            return decimal.TryParse(value, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CurrentCulture, out parsed)
                ? parsed
                : 0m;
        }
    }
}
