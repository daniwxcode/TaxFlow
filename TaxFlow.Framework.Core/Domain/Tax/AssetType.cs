using Core.Domain.Contracts;
using Core.Domain.Contracts.Abstracts;
using Core.Domain.Tax.Event;

using System.Text.RegularExpressions;
using System.Linq;

using NCalc;

namespace Core.Domain.Tax;

/// <summary>
/// Represents a type of asset in the tax domain. Acts as an aggregate root that defines
/// the expected attributes for the asset and contains behavior to manage them.
/// </summary>
public class AssetType : SoftAuditableEntity
{
    /// <summary>
    /// Backing field for the collection of expected attribute definitions. Designed to be EF Core friendly.
    /// </summary>
    private readonly List<AttributeDefinition> _expectedAttributes = new();

    /// <summary>
    /// Backing field for tax rules associated with this asset type.
    /// </summary>
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
    /// Ensures invariants such as non-empty name via <see cref="Rename(string)"/>.
    /// </summary>
    /// <param name="name">The name of the asset type.</param>
    /// <param name="description">An optional description.</param>
    /// <returns>A new <see cref="AssetType"/> instance.</returns>
    public static AssetType Create(string name, string? description = null)
    {
        var assetType = new AssetType();
        assetType.Rename(name);
        if (!string.IsNullOrWhiteSpace(description))
        {
            assetType.UpdateDescription(description!);
        }
        return assetType;
    }

    /// <summary>
    /// Rename the asset type.
    /// </summary>
    /// <param name="newName">The new name; must not be null or whitespace.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="newName"/> is null or whitespace.</exception>
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Le nom ne doit pas être vide.", nameof(newName));

        Name = newName.Trim();
        QueueDomainEvent(new AssetTypeRenamedDomainEvent(Id, newName));
    }

    /// <summary>
    /// Update the description of the asset type.
    /// </summary>
    /// <param name="newDescription">New description or null to clear.</param>
    public void UpdateDescription(string? newDescription)
    {
        Description = string.IsNullOrWhiteSpace(newDescription) ? null : newDescription.Trim();
    }

    /// <summary>
    /// Add an expected attribute definition to this asset type. Prevents duplicates by attribute key.
    /// </summary>
    /// <param name="definition">The attribute definition to add.</param>
    /// <returns>The current <see cref="AssetType"/> to allow fluent usage.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="definition"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the definition key is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when an attribute with the same key already exists.</exception>
    public AssetType AddExpectedAttribute(AttributeDefinition definition)
    {
        if (definition is null) throw new ArgumentNullException(nameof(definition));
        if (string.IsNullOrWhiteSpace(definition.Key))
            throw new ArgumentException("La clé de l'attribut attendu ne doit pas être vide.", nameof(definition));

        if (_expectedAttributes.Any(a => a.Key == definition.Key))
            throw new InvalidOperationException($"L'attribut attendu '{definition.Key}' existe déjà.");

        _expectedAttributes.Add(definition);
        return this;
    }

    /// <summary>
    /// Remove an expected attribute definition identified by instance.
    /// </summary>
    /// <param name="definition">The attribute definition to remove.</param>
    /// <returns>True if one or more definitions were removed; otherwise false.</returns>
    public bool RemoveExpectedAttribute(AttributeDefinition definition)
    {
        if (definition is null) throw new ArgumentNullException(nameof(definition));
        var removed = _expectedAttributes.RemoveAll(a => a.Key.Equals(definition.Key, StringComparison.OrdinalIgnoreCase)) > 0;
        return removed;
    }

    /// <summary>
    /// Remove an expected attribute definition by key.
    /// </summary>
    /// <param name="key">The key of the attribute to remove.</param>
    /// <returns>True if one or more definitions were removed; otherwise false.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> is null or whitespace.</exception>
    public bool RemoveExpectedAttribute(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("La clé ne doit pas être vide.", nameof(key));

        var removed = _expectedAttributes.RemoveAll(a => a.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) > 0;
        return removed;
    }

    /// <summary>
    /// Checks whether an expected attribute with the given key exists.
    /// </summary>
    /// <param name="key">Attribute key to check.</param>
    /// <returns>True if an expected attribute with the provided key exists; otherwise false.</returns>
    public bool HasExpectedAttribute(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;
        return _expectedAttributes.Any(a => a.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Adds a tax rule to the asset type.
    /// </summary>
    /// <param name="rule">Tax rule to attach.</param>
    /// <returns>The current <see cref="AssetType"/> instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="rule"/> is null.</exception>
    /// <exception cref="InvalidOperationException">If a rule with the same key already exists.</exception>
    public AssetType AddTaxRule(TaxRule rule)
    {
        if (rule is null) throw new ArgumentNullException(nameof(rule));
        if (string.IsNullOrWhiteSpace(rule.Key)) throw new ArgumentException("Rule key must not be empty", nameof(rule));
        if (_taxRules.Any(r => r.Key == rule.Key)) throw new InvalidOperationException($"A tax rule with key '{rule.Key}' already exists.");
        _taxRules.Add(rule);
        return this;
    }

    /// <summary>
    /// Remove a tax rule by key.
    /// </summary>
    /// <param name="key">Key of the rule to remove.</param>
    /// <returns>True if removed; otherwise false.</returns>
    public bool RemoveTaxRule(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key must not be empty", nameof(key));
        var removed = _taxRules.RemoveAll(r => r.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) > 0;
        return removed;
    }

    /// <summary>
    /// Evaluate a specific tax rule by key for a given set of extended attributes.
    /// Variables available to the expression are the attribute keys (trimmed) and an optional 'amount' variable.
    /// </summary>
    /// <param name="ruleKey">Rule key to evaluate.</param>
    /// <param name="attributes">Asset attributes used as variables.</param>
    /// <param name="amount">Optional base amount variable for the expression.</param>
    /// <returns>The evaluated result as decimal if numeric, otherwise null.</returns>
    public decimal? EvaluateTaxRule(string ruleKey, IEnumerable<ExtendedAttribute> attributes, decimal? amount = null)
    {
        if (string.IsNullOrWhiteSpace(ruleKey)) throw new ArgumentException("ruleKey must not be empty", nameof(ruleKey));
        var rule = _taxRules.FirstOrDefault(r => r.Key.Equals(ruleKey, StringComparison.OrdinalIgnoreCase));
        if (rule is null) return null;
        if (!rule.Enabled) return null;

        var expr = new Expression(rule.Expression, EvaluateOptions.IgnoreCase);

        // Provide 'amount' variable if present
        if (amount.HasValue)
            expr.Parameters["amount"] = (double)amount.Value;

        // Map attributes to variables by key
        foreach (var attr in attributes)
        {
            var varName = attr.Key?.Trim();
            if (string.IsNullOrWhiteSpace(varName)) continue;
            // Try cast numeric types for better evaluation, fallback to string
            if (double.TryParse(attr.Value, out var num))
                expr.Parameters[varName] = num;
            else if (bool.TryParse(attr.Value, out var b))
                expr.Parameters[varName] = b;
            else
                expr.Parameters[varName] = attr.Value;
        }

        var result = expr.Evaluate();
        if (result == null) return null;
        if (result is double d) return (decimal)d;
        if (result is int i) return i;
        if (result is decimal dec) return dec;
        if (decimal.TryParse(result.ToString(), out var parsed)) return parsed;
        return null;
    }

    /// <summary>
    /// Validates a set of extended attributes against the expectations defined on this asset type.
    /// Validation rules:
    /// <list type="bullet">
    /// <item><description>All required attributes must be present.</description></item>
    /// <item><description>Provided attribute data types must match expected data types when defined.</description></item>
    /// <item><description>Provided values must satisfy <see cref="ExtendedAttribute.IsValidValue"/>.</description></item>
    /// <item><description>If a regex pattern is defined for an attribute, the provided value must match it.</description></item>
    /// <item><description>When an attribute is an enum, the provided value must be one of the authorized enum item codes.</description></item>
    /// </list>
    /// </summary>
    /// <param name="attributes">The attributes to validate.</param>
    /// <returns>A sequence of validation error messages. Empty when validation succeeds.</returns>
    public IEnumerable<string> ValidateAttributes(IEnumerable<ExtendedAttribute> attributes)
    {
        if (attributes is null) throw new ArgumentNullException(nameof(attributes));
        var errors = new List<string>();

        var byKey = attributes.ToDictionary(a => a.Key, a => a, StringComparer.OrdinalIgnoreCase);

        foreach (var expected in _expectedAttributes)
        {
            if (expected.IsRequired && !byKey.ContainsKey(expected.Key))
            {
                errors.Add($"Attribut requis manquant: '{expected.Key}'.");
                continue;
            }
            if (byKey.TryGetValue(expected.Key, out var provided))
            {
                if (expected.DataType is not null && provided.DataTypeValue != expected.DataType.Value)
                {
                    errors.Add($"Type invalide pour '{expected.Key}': attendu {expected.DataType!.ToString()}, obtenu {provided.DataType.ToString()}.");
                }

                if (!provided.IsValidValue())
                {
                    errors.Add($"Valeur invalide pour '{expected.Key}' au format {provided.DataType.ToString()}.");
                }

                if (!string.IsNullOrWhiteSpace(expected.RegexPattern))
                {
                    try
                    {
                        if (!Regex.IsMatch(provided.Value ?? string.Empty, expected.RegexPattern!))
                        {
                            errors.Add($"La valeur de '{expected.Key}' ne respecte pas le motif requis.");
                        }
                    }
                    catch (ArgumentException)
                    {
                        errors.Add($"Motif Regex invalide pour la définition d'attribut '{expected.Key}'.");
                    }
                }

                if (expected.DataType == Core.Domain.Enums.AttributeDataType.Enum)
                {
                    var enumDef = expected.EnumDefinition;
                    if (enumDef is null || enumDef.Items == null || !enumDef.Items.Any())
                    {
                        errors.Add($"Définition d'enum manquante pour l'attribut '{expected.Key}'.");
                    }
                    else
                    {
                        var allowed = enumDef.Items.Select(i => i.Code).Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c!.Trim()).ToList();
                        var providedValue = (provided.Value ?? string.Empty).Trim();
                        if (!allowed.Any(a => string.Equals(a, providedValue, StringComparison.OrdinalIgnoreCase)))
                        {
                            var allowedList = string.Join(", ", allowed);
                            errors.Add($"Valeur invalide pour '{expected.Key}': '{providedValue}' n'est pas dans les valeurs autorisées [{allowedList}].");
                        }
                    }
                }
            }

        }

        return errors;
    }
}
