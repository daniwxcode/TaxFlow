using Core.Domain.Contracts.Abstracts;

using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Contracts;

public class ExtendedAttribute: SoftAuditableEntity, ITemporalValiditable
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
}