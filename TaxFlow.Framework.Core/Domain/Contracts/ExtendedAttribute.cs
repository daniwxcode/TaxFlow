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
    /// Attribute value as stored (string representation).
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
    /// Typed view of the data type.
    /// </summary>
    [NotMapped]
    public AttributeDataType DataType
    {
        get => AttributeDataType.FromValue(DataTypeValue);
        protected set => DataTypeValue = value.Value;
    }

    /// <summary>
    /// Start of temporal validity.
    /// </summary>
    public DateTimeOffset ValidFrom { get; set; }

    /// <summary>
    /// End of temporal validity (nullable for open-ended).
    /// </summary>
    public DateTimeOffset? ValidTo { get; set; }

    /// <summary>
    /// Protected parameterless constructor for ORM.
    /// </summary>
    protected ExtendedAttribute() { }

    /// <summary>
    /// Factory method to create a new ExtendedAttribute instance.
    /// </summary>
    public static ExtendedAttribute Create(
        string key,
        string value,
        AttributeDataType dataType,
        bool isRequired = false,
        DateTimeOffset? validFrom = null,
        DateTimeOffset? validTo = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

        return new ExtendedAttribute
        {
            Key = key.Trim(),
            Value = value ?? string.Empty,
            DataType = dataType,
            IsRequired = isRequired,
            ValidFrom = validFrom ?? DateTimeOffset.UtcNow,
            ValidTo = validTo
        };
    }

    /// <summary>
    /// Update value and metadata for the attribute.
    /// </summary>
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
    public bool IsValidValue()
    {
        if (string.IsNullOrWhiteSpace(Value))
            return !IsRequired;

        return DataType.Value switch
        {
            var v when v == AttributeDataType.Number.Value => double.TryParse(Value, out _),
            var v when v == AttributeDataType.Boolean.Value => bool.TryParse(Value, out _),
            var v when v == AttributeDataType.Date.Value => DateTimeOffset.TryParse(Value, out _),
            var v when v == AttributeDataType.Enum.Value => true,
            var v when v == AttributeDataType.Json.Value => IsValidJson(Value),
            _ => true,
        };
    }

    /// <summary>
    /// Checks if the attribute is currently valid at the given date.
    /// </summary>
    public bool IsValidAt(DateTimeOffset date)
    {
        return ValidFrom <= date && (ValidTo == null || ValidTo >= date);
    }

    private static bool IsValidJson(string value)
    {
        try
        {
            using var _ = System.Text.Json.JsonDocument.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}