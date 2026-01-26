using Core.Domain.Contracts;
using Core.Domain.Contracts.Abstracts;
using Core.Domain.Contracts.Validation;
using Core.Domain.Enums;
using Core.Domain.Localization;
using Core.Domain.Tax.Calculation;
using Core.Domain.Tax.Calculation.Services;
using Core.Domain.Tax.Events;

namespace Core.Domain.Tax.Assets;

/// <summary>
/// Represents a type of asset in the tax domain. Acts as an aggregate root that defines
/// the expected attributes for the asset and contains behavior to manage them.
/// </summary>
public class AssetType : SoftAuditableEntity
{
    private readonly List<AttributeDefinition> _expectedAttributes = new();
    private readonly List<TaxRule> _taxRules = new();

    /// <summary>
    /// Gets the name of the asset type.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets an optional description for the asset type.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the liquidation mode applied to assets of this type.
    /// </summary>
    public LiquidationMode LiquidationMode { get; private set; } = LiquidationMode.Individual;

    /// <summary>
    /// Gets the read-only collection of attribute definitions expected for this asset type.
    /// </summary>
    public IReadOnlyCollection<AttributeDefinition> ExpectedAttributes => _expectedAttributes.AsReadOnly();

    /// <summary>
    /// Gets the read-only collection of tax rules defined for this asset type.
    /// </summary>
    public IReadOnlyCollection<TaxRule> TaxRules => _taxRules.AsReadOnly();

    /// <summary>
    /// Protected parameterless constructor for EF Core and other infrastructure.
    /// </summary>
    protected AssetType() { }

    /// <summary>
    /// Factory method to create a new <see cref="AssetType"/> with the specified name and optional description.
    /// </summary>
    public static AssetType Create(string name, string? description = null, LiquidationMode liquidationMode = LiquidationMode.Individual)
    {
        var assetType = new AssetType();
        assetType.Rename(name);
        assetType.UpdateLiquidationMode(liquidationMode);

        if (!string.IsNullOrWhiteSpace(description))
        {
            assetType.UpdateDescription(description);
        }

        return assetType;
    }

    /// <summary>
    /// Rename the asset type.
    /// </summary>
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException(ExceptionMessages.NameCannotBeEmpty.Format(), nameof(newName));
        }

        Name = newName.Trim();
        QueueDomainEvent(new AssetTypeRenamedDomainEvent(Id, newName));
    }

    /// <summary>
    /// Update the description of the asset type.
    /// </summary>
    public void UpdateDescription(string? newDescription)
    {
        Description = string.IsNullOrWhiteSpace(newDescription) ? null : newDescription.Trim();
    }

    /// <summary>
    /// Update the liquidation mode of the asset type.
    /// </summary>
    public void UpdateLiquidationMode(LiquidationMode mode)
    {
        if (!Enum.IsDefined(typeof(LiquidationMode), mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        LiquidationMode = mode;
    }

    #region Expected Attributes Management

    /// <summary>
    /// Add an expected attribute definition to this asset type.
    /// </summary>
    public AssetType AddExpectedAttribute(AttributeDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        if (string.IsNullOrWhiteSpace(definition.Key))
        {
            throw new ArgumentException(ExceptionMessages.ExpectedAttributeKeyCannotBeEmpty.Format(), nameof(definition));
        }

        if (HasExpectedAttribute(definition.Key))
        {
            throw new InvalidOperationException(ExceptionMessages.ExpectedAttributeAlreadyExists.Format(("attributeKey", definition.Key)));
        }

        _expectedAttributes.Add(definition);
        return this;
    }

    /// <summary>
    /// Remove an expected attribute definition by instance.
    /// </summary>
    public bool RemoveExpectedAttribute(AttributeDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        return RemoveExpectedAttribute(definition.Key);
    }

    /// <summary>
    /// Remove an expected attribute definition by key.
    /// </summary>
    public bool RemoveExpectedAttribute(string key)
    {
        return string.IsNullOrWhiteSpace(key)
            ? throw new ArgumentException(ExceptionMessages.KeyCannotBeEmpty.Format(), nameof(key))
            : _expectedAttributes.RemoveAll(a => a.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) > 0;
    }

    /// <summary>
    /// Checks whether an expected attribute with the given key exists.
    /// </summary>
    public bool HasExpectedAttribute(string key)
    {
        return !string.IsNullOrWhiteSpace(key) && _expectedAttributes.Any(a => a.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region Tax Rules Management

    /// <summary>
    /// Adds a tax rule to the asset type.
    /// </summary>
    public AssetType AddTaxRule(TaxRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        if (string.IsNullOrWhiteSpace(rule.Key))
        {
            throw new ArgumentException(ExceptionMessages.TaxRuleKeyCannotBeEmpty.Format(), nameof(rule));
        }

        if (_taxRules.Any(r => r.Key.Equals(rule.Key, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(ExceptionMessages.TaxRuleAlreadyExists.Format(("ruleKey", rule.Key)));
        }

        TaxRuleExpressionValidator.Validate(rule);

        _taxRules.Add(rule);
        return this;
    }

    /// <summary>
    /// Remove a tax rule by key.
    /// </summary>
    public bool RemoveTaxRule(string key)
    {
        return string.IsNullOrWhiteSpace(key)
            ? throw new ArgumentException(ExceptionMessages.KeyCannotBeEmpty.Format(), nameof(key))
            : _taxRules.RemoveAll(r => r.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) > 0;
    }

    /// <summary>
    /// Find a tax rule by key.
    /// </summary>
    public TaxRule? FindTaxRule(string ruleKey)
    {
        return string.IsNullOrWhiteSpace(ruleKey)
            ? null
            : _taxRules.FirstOrDefault(r => r.Key.Equals(ruleKey, StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region Tax Rule Evaluation

    /// <summary>
    /// Evaluate a specific tax rule by key for a given set of extended attributes.
    /// </summary>
    public decimal? EvaluateTaxRule(string ruleKey, IEnumerable<ExtendedAttribute> attributes, decimal? amount = null)
    {
        return EvaluateTaxRuleDetailed(ruleKey, attributes, amount).Value;
    }

    /// <summary>
    /// Evaluate a specific tax rule and return a detailed result.
    /// </summary>
    public TaxRuleEvaluationResult EvaluateTaxRuleDetailed(
        string ruleKey,
        IEnumerable<ExtendedAttribute> attributes,
        decimal? amount = null)
    {
        if (string.IsNullOrWhiteSpace(ruleKey))
        {
            return TaxRuleEvaluationResult.CreateFailure(ruleKey ?? string.Empty, "ruleKey must not be empty.");
        }

        TaxRule? rule = FindTaxRule(ruleKey);
        return rule is null
            ? TaxRuleEvaluationResult.CreateFailure(ruleKey, $"Rule '{ruleKey}' not found.")
            : ((DefaultTaxRuleEvaluator)DefaultTaxRuleEvaluator.Default).Evaluate(rule, attributes, _expectedAttributes, amount);
    }

    #endregion

    #region Attribute Validation

    /// <summary>
    /// Validates a set of extended attributes against the expectations defined on this asset type.
    /// </summary>
    /// <returns>A validation result with structured errors.</returns>
    public ValidationResult ValidateAttributesResult(IEnumerable<ExtendedAttribute> attributes)
    {
        return AttributeValidator.Default.Validate(attributes, _expectedAttributes);
    }

    /// <summary>
    /// Validates a set of extended attributes against the expectations defined on this asset type.
    /// </summary>
    /// <returns>A sequence of validation error messages for backward compatibility.</returns>
    public IEnumerable<string> ValidateAttributes(IEnumerable<ExtendedAttribute> attributes)
    {
        return ValidateAttributesResult(attributes).ToMessages();
    }

    #endregion
}
