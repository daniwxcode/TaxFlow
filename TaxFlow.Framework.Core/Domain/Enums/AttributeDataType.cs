using Ardalis.SmartEnum;

namespace Core.Domain.Enums;

public sealed class AttributeDataType : SmartEnum<AttributeDataType>
{
    public static readonly AttributeDataType String = new AttributeDataType("String", 1);
    public static readonly AttributeDataType Number = new AttributeDataType("Number", 2);
    public static readonly AttributeDataType Boolean = new AttributeDataType("Boolean", 3);
    public static readonly AttributeDataType Date = new AttributeDataType("Date", 4);
    public static readonly AttributeDataType Enum = new AttributeDataType("Enum", 5);
    public static readonly AttributeDataType Json = new AttributeDataType("Json", 6);

    private AttributeDataType(string name, int value) : base(name, value) { }
}
