using Core.Domain.Contracts.Abstracts;
using Core.Domain.Enums;

using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Contracts;

public class ExtendedAttribute : SoftAuditableEntity, ITemporalValiditable
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DataTypeValue { get; set; }
    public bool IsRequired { get; set; }

    [NotMapped]
    public AttributeDataType DataType
    {
        get => AttributeDataType.FromValue(DataTypeValue);
        set => DataTypeValue = value.Value;
    }
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }
    public bool IsValidValue()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            return !IsRequired; // Si requis, la valeur ne doit pas être vide
        }

        return DataType.Name switch
        {
            "Number" => double.TryParse(Value, out _),
            "Boolean" => bool.TryParse(Value, out _),
            "Date" => DateTimeOffset.TryParse(Value, out _),
            "Enum" => true, // Pour enum, tu peux ajouter une validation spécifique si tu as la liste des valeurs possibles
            "Json" => IsValidJson(Value),
            _ => true, // String et autres types acceptent toute valeur non vide
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