using Core.Domain.Contracts;
using Core.Domain.Contracts.Abstracts;
using Core.Domain.Contracts.Validation;
using Core.Domain.Enums;
using Core.Domain.Localization;
using Core.Domain.Tax.Calculation;
using Core.Domain.Tax.Calculation.Services;

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Core.Domain.Tax.Assets;

/// <summary>
/// Represents an asset subject to taxation.
/// </summary>
public class TaxableAsset : ExtendedTemporalSoftAuditableEntity
{
    /// <summary>
    /// The asset type describing the expected attributes and rules for this asset.
    /// </summary>
    public AssetType AssetType { get; private set; } = default!;

    /// <summary>
    /// Foreign key identifier of the asset type.
    /// </summary>
    public Guid AssetTypeId { get; private set; }

    /// <summary>
    /// Protected parameterless constructor for EF Core and infrastructure.
    /// </summary>
    protected TaxableAsset() { }

    /// <summary>
    /// Factory method to create a new <see cref="TaxableAsset"/> instance.
    /// </summary>
    public static TaxableAsset Create(AssetType assetType, Collection<ExtendedAttribute> attributes)
    {
        ArgumentNullException.ThrowIfNull(assetType);

        var validationResult = assetType.ValidateAttributesResult(attributes);
        if (validationResult.HasErrors)
            throw new ArgumentException(ExceptionMessages.AttributeValidationFailed.Format(("errorMessage", validationResult.ErrorMessage)));

        return new TaxableAsset
        {
            AssetType = assetType,
            AssetTypeId = assetType.Id,
            _attributes = attributes.ToList()
        };
    }

    #region Tax Calculation

    /// <summary>
    /// Calculate tax lines for the current asset using the tax rules defined on its AssetType.
    /// </summary>
    public IReadOnlyCollection<TaxLine> CalculateTaxLines(decimal? baseAmount = null, DateTimeOffset? forDate = null)
    {
        EnsureAssetTypeSet();

        forDate ??= DateTimeOffset.UtcNow;
        var effectiveAttributes = GetEffectiveAttributes(forDate.Value);
        var effectiveRules = GetEffectiveRules(forDate.Value);

        var lines = new List<TaxLine>();
        foreach (var rule in effectiveRules)
        {
            var value = AssetType.EvaluateTaxRule(rule.Key ?? string.Empty, effectiveAttributes, baseAmount) ?? 0m;
            lines.Add(new TaxLine(rule.Key ?? string.Empty, rule.Label ?? string.Empty, value));
        }

        return lines.AsReadOnly();
    }

    /// <summary>
    /// Calculate taxes using the optimized engine with diagnostics.
    /// </summary>
    public TaxCalculationResult CalculateTaxes(TaxEngineOptions? options = null)
    {
        return TaxEngine.Evaluate(this, options);
    }

    /// <summary>
    /// Calculate taxes for a period using the optimized engine with diagnostics.
    /// </summary>
    public TaxCalculationResult CalculateTaxesForPeriod(
        DateTimeOffset from,
        DateTimeOffset to,
        int daysInYear = 365,
        TaxEngineOptions? options = null)
    {
        return TaxEngine.EvaluateForPeriod(this, from, to, daysInYear, options);
    }

    /// <summary>
    /// Calculate tax lines for a period and apply simple prorata.
    /// </summary>
    public TaxCalculationResult CalculateTaxLinesForPeriod(
        DateTimeOffset from,
        DateTimeOffset to,
        decimal? baseAmount = null,
        string? currency = null,
        int? precision = null,
        MidpointRounding? rounding = null,
        int daysInYear = 365)
    {
        ValidatePeriod(from, to, daysInYear);

        var baseLines = CalculateTaxLines(baseAmount, from);
        var factor = CalculateProrataFactor(from, to, daysInYear);

        var prorated = baseLines
            .Select(l => new TaxLine(l.Key, l.Label, l.Amount * factor, currency, precision, rounding))
            .ToList();

        return new TaxCalculationResult(prorated, currency, precision, rounding);
    }

    /// <summary>
    /// Calculate tax lines and return totals with optional rounding and currency.
    /// </summary>
    public TaxCalculationResult CalculateTaxLinesDetailed(
        decimal? baseAmount = null,
        DateTimeOffset? forDate = null,
        string? currency = null,
        int? precision = null,
        MidpointRounding? rounding = null)
    {
        var lines = CalculateTaxLines(baseAmount, forDate)
            .Select(l => new TaxLine(l.Key, l.Label, l.Amount, currency, precision, rounding))
            .ToList();

        return new TaxCalculationResult(lines, currency, precision, rounding);
    }

    #endregion

    #region Attribute Management

    /// <summary>
    /// Add or update an attribute with validation.
    /// </summary>
    public ExtendedAttribute AddOrUpdateAttributeValidated(
        string key,
        string value,
        AttributeDataType dataType,
        bool isRequired = false,
        DateTimeOffset? validFrom = null,
        DateTimeOffset? validTo = null)
    {
        EnsureAssetTypeSet();

        var candidate = ExtendedAttribute.Create(key, value, dataType, isRequired, validFrom ?? DateTimeOffset.UtcNow, validTo);
        var updatedAttributes = BuildUpdatedAttributeList(key, candidate);

        var validationResult = AssetType.ValidateAttributesResult(updatedAttributes);
        if (validationResult.HasErrors)
            throw new ArgumentException(ExceptionMessages.AttributeValidationFailed.Format(("errorMessage", validationResult.ErrorMessage)));

        return ApplyAttributeChange(key, candidate);
    }

    #endregion

    #region Private Helpers

    private void EnsureAssetTypeSet()
    {
        if (AssetType is null)
            throw new InvalidOperationException(ExceptionMessages.AssetTypeMustBeSet.Format());
    }

    private List<ExtendedAttribute> GetEffectiveAttributes(DateTimeOffset forDate)
    {
        return _attributes
            .Where(a => a.ValidFrom <= forDate && (a.ValidTo == null || a.ValidTo >= forDate))
            .ToList();
    }

    private IEnumerable<TaxRule> GetEffectiveRules(DateTimeOffset forDate)
    {
        return AssetType.TaxRules
            .Where(r => r.Enabled && r.ValidFrom <= forDate && (r.ValidTo == null || r.ValidTo >= forDate));
    }

    private static void ValidatePeriod(DateTimeOffset from, DateTimeOffset to, int daysInYear)
    {
        if (to < from)
            throw new ArgumentException(ExceptionMessages.EndDateMustBeGreaterOrEqual.Format(), nameof(to));

        if (daysInYear <= 0)
            throw new ArgumentOutOfRangeException(nameof(daysInYear), ExceptionMessages.DaysInYearMustBePositive.Format());
    }

    private static decimal CalculateProrataFactor(DateTimeOffset from, DateTimeOffset to, int daysInYear)
    {
        var days = (to.Date - from.Date).TotalDays + 1;
        return (decimal)days / daysInYear;
    }

    private List<ExtendedAttribute> BuildUpdatedAttributeList(string key, ExtendedAttribute candidate)
    {
        var updated = _attributes.ToList();
        var existing = updated.FirstOrDefault(a => a.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
            updated.Remove(existing);

        updated.Add(candidate);
        return updated;
    }

    private ExtendedAttribute ApplyAttributeChange(string key, ExtendedAttribute candidate)
    {
        var existing = _attributes.FirstOrDefault(a => a.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
        {
            existing.UpdateValue(candidate.Value, candidate.DataType, candidate.IsRequired);
            existing.ValidFrom = candidate.ValidFrom;
            existing.ValidTo = candidate.ValidTo;
            return existing;
        }

        _attributes.Add(candidate);
        return candidate;
    }

    #endregion
}
