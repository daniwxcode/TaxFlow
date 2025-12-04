using Core.Domain.Contracts.Abstracts;
using Core.Domain.Enums;

namespace Core.Domain.Contracts;

/// <summary>
/// Represents one item inside an EnumDefinition.
/// </summary>
public class EnumItem : AuditableEntity
{
    /// <summary>
    /// The code used to identify the enum item (stored value).
    /// </summary>
    public string Code { get; internal set; } = default!;

    /// <summary>
    /// Human-readable label for the item.
    /// </summary>
    public string Label { get; internal set; } = default!;

    /// <summary>
    /// Order for presentation or sorting within the enum.
    /// </summary>
    public int Order { get; internal set; }

    /// <summary>
    /// Foreign key to the owning <see cref="EnumDefinition"/> - used by persistence.
    /// </summary>
    public int EnumDefinitionId { get; internal set; }

    /// <summary>
    /// Navigation property to the owning <see cref="EnumDefinition"/>.
    /// </summary>
    public EnumDefinition EnumDefinition { get; internal set; } = default!;
}

/// <summary>
/// Describes an attribute expected by an asset type: key, label, datatype and optional enum definition or regex.
/// </summary>
public class AttributeDefinition: AuditableEntity
{
    /// <summary>
    /// Attribute key used as identifier.
    /// </summary>
    public string Key { get; internal set; }

    /// <summary>
    /// Human-readable label for the attribute.
    /// </summary>
    public string Label { get; internal set; }= string.Empty;

    /// <summary>
    /// Expected data type for the attribute.
    /// </summary>
    public AttributeDataType DataType { get; internal set; } = AttributeDataType.String;

    /// <summary>
    /// Whether the attribute is required for assets of the parent type.
    /// </summary>
    public bool IsRequired { get; internal set; }

    /// <summary>
    /// Optional foreign key to an enum definition when DataType is Enum.
    /// </summary>
    public int? EnumDefinitionId { get; internal set; }

    /// <summary>
    /// Optional navigation to the enum definition when DataType is Enum.
    /// </summary>
    public EnumDefinition? EnumDefinition { get; internal set; }

    /// <summary>
    /// Optional regex pattern used to validate string values for this attribute.
    /// </summary>
    public string? RegexPattern { get; private set; }

    /// <summary>
    /// Factory to create an attribute definition while enforcing required fields.
    /// </summary>
    /// <param name="key">Attribute key.</param>
    /// <param name="label">Human-readable label.</param>
    /// <param name="dataType">Data type.</param>
    /// <param name="definition">Optional enum definition.</param>
    /// <param name="isRequired">Whether the attribute is required.</param>
    /// <param name="regexPattern">Optional regex pattern.</param>
    /// <returns>A configured <see cref="AttributeDefinition"/> instance.</returns>
    public static AttributeDefinition Create(string key, string label, AttributeDataType dataType, bool isRequired=false
        ,string regexPattern= null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("La clé de l'attribut ne doit pas être vide.", nameof(key));
        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Le label de l'attribut ne doit pas être vide.", nameof(label));
        return new AttributeDefinition
        {
            Key = key.Trim(),
            Label = label.Trim(),
            DataType = dataType,
            IsRequired = isRequired,
            RegexPattern = regexPattern
        };
    }

    public static AttributeDefinition Create(EnumDefinition enumDefinition,bool isRequired=true)
    {
        if (enumDefinition == null)
            throw new ArgumentNullException(nameof(enumDefinition), "La définition d'énumération ne doit pas être nulle.");
        return new AttributeDefinition
        {
            Key = enumDefinition.Key,
            Label = enumDefinition.Label,
            DataType = AttributeDataType.Enum,
            EnumDefinition = enumDefinition,
            IsRequired = isRequired,
            // Utiliser les codes des items d'énumération pour la validation des valeurs d'attributs
            RegexPattern = enumDefinition.BuildCodeRegex()
        };
    }

    /// <summary>
    /// Update the label of the attribute definition.
    /// </summary>
    /// <param name="label">New label.</param>
    /// <returns>The same instance for fluent usage.</returns>
    public AttributeDefinition UpdateLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label)) throw new ArgumentException("Le label ne doit pas être vide.", nameof(label));
        Label = label.Trim();
        return this;
    }

    /// <summary>
    /// Set or clear the regex validation pattern for this attribute.
    /// </summary>
    /// <param name="pattern">Regex pattern or null to clear.</param>
    /// <returns>The same instance for fluent usage.</returns>
    public AttributeDefinition SetRegexPattern(string? pattern)
    {
        RegexPattern = string.IsNullOrWhiteSpace(pattern) ? null : pattern;
        return this;
    }
}
