using Core.Domain.Contracts.Abstracts;
using Core.Domain.Enums;

using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Contracts;

/// <summary>
/// Represents an extendable attribute attached to an entity with temporal validity and basic type validation.
/// </summary>
public class ExtendedAttribute : SoftAuditableEntity, ITemporalValiditable
{
    /// <summary>
    /// Attribute key.
    /// </summary>
    public string Key { get; protected set; } = string.Empty;

    /// <summary>
    /// Attribute value as stored (string representation). Conversion/validation depends on <see cref="DataType"/>.
    /// </summary>
    public string Value { get; protected set; } = string.Empty;

    /// <summary>
    /// Backing integer for <see cref="DataType"/> to facilitate persistence.
    /// </summary>
    public int DataTypeValue { get; protected set; }

    /// <summary>
    /// Whether the attribute is required.
    /// </summary>
    public bool IsRequired { get; protected set; }

    /// <summary>
    /// Typed view of the data type stored in <see cref="DataTypeValue"/>.
    /// </summary>
    [NotMapped]
    public AttributeDataType DataType
    {
        get => AttributeDataType.FromValue(DataTypeValue);
        protected set => DataTypeValue = value.Value;
    }

    /// <summary>
    /// Start of temporal validity for this attribute.
    /// </summary>
    public DateTimeOffset ValidFrom { get; set; }

    /// <summary>
    /// End of temporal validity for this attribute (nullable for open-ended validity).
    /// </summary>
    public DateTimeOffset? ValidTo { get; set; }

    /// <summary>
    /// Protected parameterless constructor for ORM and infrastructure.
    /// </summary>
    protected ExtendedAttribute() { }

    /// <summary>
    /// Factory method to create a new ExtendedAttribute instance with validation for the key.
    /// </summary>
    /// <param name="key">Attribute key; must not be empty.</param>
    /// <param name="value">Attribute value.</param>
    /// <param name="dataType">Attribute data type.</param>
    /// <param name="isRequired">Whether the attribute is required.</param>
    /// <param name="validFrom">Optional validity start time; defaults to UTC now.</param>
    /// <param name="validTo">Optional validity end time.</param>
    /// <returns>A new <see cref="ExtendedAttribute"/> instance.</returns>
    public static ExtendedAttribute Create(string key, string value, AttributeDataType dataType, bool isRequired = false, DateTimeOffset? validFrom = null, DateTimeOffset? validTo = null)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key must not be empty", nameof(key));
        var attr = new ExtendedAttribute
        {
            Key = key.Trim(),
            Value = value ?? string.Empty,
            DataType = dataType,
            IsRequired = isRequired,
            ValidFrom = validFrom ?? DateTimeOffset.UtcNow,
            ValidTo = validTo
        };
        return attr;
    }

    /// <summary>
    /// Update value and metadata for the attribute in a single domain-intent method.
    /// </summary>
    /// <param name="value">The new value.</param>
    /// <param name="dataType">The data type for the value.</param>
    /// <param name="isRequired">Whether the attribute is required.</param>
    public void UpdateValue(string value, AttributeDataType dataType, bool isRequired = false)
    {
        Value = value ?? string.Empty;
        DataType = dataType;
        IsRequired = isRequired;
        ValidFrom = DateTimeOffset.UtcNow;
        ValidTo = null;
    }

    /// <summary>
    /// Validates the stored string value against the <see cref="DataType"/>.
    /// </summary>
    /// <returns>True if the value is valid given the data type and required flag; otherwise false.</returns>
    public bool IsValidValue()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            return !IsRequired;
        }

        return DataType.Name switch
        {
            "Number" => double.TryParse(Value, out _),
            "Boolean" => bool.TryParse(Value, out _),
            "Date" => DateTimeOffset.TryParse(Value, out _),
            "Enum" => true,
            "Json" => IsValidJson(Value),
            _ => true,
        };

    }
    private bool IsValidJson(string value)
    {
        try
        {
            var _ = System.Text.Json.JsonDocument.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}