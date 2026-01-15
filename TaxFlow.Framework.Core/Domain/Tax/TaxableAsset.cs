using Core.Domain.Contracts;
using Core.Domain.Contracts.Abstracts;
using Core.Domain.Enums;

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Core.Domain.Tax;

/// <summary>
/// Represents an asset subject to taxation. Contains a reference to its asset type and a collection of extended attributes.
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
    /// Factory method to create a new <see cref="TaxableAsset"/> instance while enforcing attribute validation against the provided <see cref="AssetType"/>.
    /// </summary>
    /// <param name="assetType">The asset type to associate with this asset.</param>
    /// <param name="attributes">Collection of extended attributes for the asset.</param>
    /// <returns>A new <see cref="TaxableAsset"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assetType"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when attribute validation fails.</exception>
    public static TaxableAsset Create(AssetType assetType, Collection<ExtendedAttribute> attributes)
    {
        if (assetType is null) throw new ArgumentNullException(nameof(assetType));

        var validationResult = assetType.ValidateAttributes(attributes);
        if (validationResult.Any())
        {
            var errorMessages = string.Join("; ", validationResult.ToArray());
            throw new ArgumentException($"Attributes validation failed: {errorMessages}");
        }

        var taxableAsset = new TaxableAsset
        {
            AssetType = assetType,
            AssetTypeId = assetType.Id,
            _attributes = attributes.ToList()
        };
        return taxableAsset;
    }

    /// <summary>
    /// Calculate tax lines for the current asset using the tax rules defined on its AssetType.
    /// Returns one line per enabled rule. If a rule evaluation returns null the amount is treated as 0.
    /// </summary>
    /// <param name="baseAmount">Optional base amount provided to rule expressions via the 'amount' variable.</param>
    /// <param name="forDate">Optional date to filter temporal attributes; if null current UTC is used.</param>
    /// <returns>Read-only collection of calculated tax lines.</returns>
    public IReadOnlyCollection<TaxLine> CalculateTaxLines(decimal? baseAmount = null, DateTimeOffset? forDate = null)
    {
        if (AssetType is null) throw new InvalidOperationException("AssetType must be set to calculate taxes.");

        forDate ??= DateTimeOffset.UtcNow;

        var effectiveAttributes = _attributes
            .Where(a => a.ValidFrom <= forDate && (a.ValidTo == null || a.ValidTo >= forDate))
            .ToList();

        var lines = new List<TaxLine>();

        var effectiveRules = AssetType.TaxRules
            .Where(r => r.Enabled && r.ValidFrom <= forDate && (r.ValidTo == null || r.ValidTo >= forDate));

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
    /// <param name="options">Optional engine options.</param>
    /// <returns>Tax calculation result with totals and diagnostics.</returns>
    public TaxCalculationResult CalculateTaxes(TaxEngineOptions? options = null)
    {
        return TaxEngine.Evaluate(this, options);
    }

    /// <summary>
    /// Calculate taxes for a period using the optimized engine with diagnostics.
    /// </summary>
    /// <param name="from">Period start (inclusive).</param>
    /// <param name="to">Period end (inclusive).</param>
    /// <param name="daysInYear">Proration basis (default 365).</param>
    /// <param name="options">Optional engine options.</param>
    /// <returns>Tax calculation result with totals and diagnostics.</returns>
    public TaxCalculationResult CalculateTaxesForPeriod(DateTimeOffset from, DateTimeOffset to, int daysInYear = 365, TaxEngineOptions? options = null)
    {
        return TaxEngine.EvaluateForPeriod(this, from, to, daysInYear, options);
    }

    /// <summary>
    /// Calculate tax lines for a period and apply simple prorata based on number of days.
    /// </summary>
    /// <param name="from">Period start (inclusive).</param>
    /// <param name="to">Period end (inclusive).</param>
    /// <param name="baseAmount">Optional base amount provided to rule expressions via the 'amount' variable.</param>
    /// <param name="currency">Optional currency code to attach to lines.</param>
    /// <param name="precision">Optional decimal precision for rounding.</param>
    /// <param name="rounding">Optional rounding mode.</param>
    /// <param name="daysInYear">Proration basis (default 365).</param>
    /// <returns>Detailed calculation result with totals.</returns>
    public TaxCalculationResult CalculateTaxLinesForPeriod(DateTimeOffset from, DateTimeOffset to, decimal? baseAmount = null, string? currency = null, int? precision = null, MidpointRounding? rounding = null, int daysInYear = 365)
    {
        if (to < from) throw new ArgumentException("The end date must be greater than or equal to the start date.", nameof(to));
        if (daysInYear <= 0) throw new ArgumentOutOfRangeException(nameof(daysInYear), "daysInYear must be greater than 0.");

        var baseLines = CalculateTaxLines(baseAmount, from);
        var days = (to.Date - from.Date).TotalDays + 1;
        var factor = (decimal)days / daysInYear;

        var prorated = baseLines
            .Select(l => new TaxLine(l.Key, l.Label, l.Amount * factor, currency, precision, rounding))
            .ToList();

        return new TaxCalculationResult(prorated, currency, precision, rounding);
    }

    /// <summary>
    /// Calculate tax lines and return totals with optional rounding and currency.
    /// </summary>
    /// <param name="baseAmount">Optional base amount provided to rule expressions via the 'amount' variable.</param>
    /// <param name="forDate">Optional date to filter temporal attributes; if null current UTC is used.</param>
    /// <param name="currency">Optional currency code to attach to lines.</param>
    /// <param name="precision">Optional decimal precision for rounding.</param>
    /// <param name="rounding">Optional rounding mode.</param>
    /// <returns>Detailed calculation result with totals.</returns>
    public TaxCalculationResult CalculateTaxLinesDetailed(decimal? baseAmount = null, DateTimeOffset? forDate = null, string? currency = null, int? precision = null, MidpointRounding? rounding = null)
    {
        var lines = CalculateTaxLines(baseAmount, forDate)
            .Select(l => new TaxLine(l.Key, l.Label, l.Amount, currency, precision, rounding))
            .ToList();

        return new TaxCalculationResult(lines, currency, precision, rounding);
    }

    /// <summary>
    /// Add or update an attribute and validate the full attribute set against the associated AssetType.
    /// </summary>
    /// <param name="key">Attribute key.</param>
    /// <param name="value">Attribute value.</param>
    /// <param name="dataType">Attribute data type.</param>
    /// <param name="isRequired">Whether the attribute is required.</param>
    /// <param name="validFrom">Optional validity start time; defaults to UTC now.</param>
    /// <param name="validTo">Optional validity end time.</param>
    /// <returns>The added or updated attribute.</returns>
    public ExtendedAttribute AddOrUpdateAttributeValidated(string key, string value, AttributeDataType dataType, bool isRequired = false, DateTimeOffset? validFrom = null, DateTimeOffset? validTo = null)
    {
        if (AssetType is null) throw new InvalidOperationException("AssetType must be set to update attributes.");

        var updated = _attributes.ToList();
        var existing = updated.FirstOrDefault(a => a.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        var candidate = ExtendedAttribute.Create(key, value, dataType, isRequired, validFrom ?? DateTimeOffset.UtcNow, validTo);

        if (existing != null)
        {
            updated.Remove(existing);
        }

        updated.Add(candidate);

        var validation = AssetType.ValidateAttributes(updated);
        if (validation.Any())
        {
            var errorMessages = string.Join("; ", validation.ToArray());
            throw new ArgumentException($"Attributes validation failed: {errorMessages}");
        }

        if (existing != null)
        {
            existing.UpdateValue(value, dataType, isRequired);
            existing.ValidFrom = candidate.ValidFrom;
            existing.ValidTo = candidate.ValidTo;
            return existing;
        }

        _attributes.Add(candidate);
        return candidate;
    }

}
