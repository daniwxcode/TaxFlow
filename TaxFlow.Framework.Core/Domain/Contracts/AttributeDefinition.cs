using Core.Domain.Enums;

namespace Core.Domain.Contracts;

public class AttributeDefinition
{
    public Guid Id { get; set; }
    public string Key { get; set; } 
    public string ShortCode { get; set; } = string.Empty;
    public string Label { get; set; }= string.Empty;
    public AttributeDataType DataType { get; set; } = AttributeDataType.String;
    public bool IsRequired { get; set; }
}
