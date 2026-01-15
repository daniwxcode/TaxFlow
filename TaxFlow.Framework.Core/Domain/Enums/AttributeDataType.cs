using Ardalis.SmartEnum;

namespace Core.Domain.Enums;
/// <summary>
/// Defines the data types available for attributes.
/// </summary>
public sealed class AttributeDataType : SmartEnum<AttributeDataType>
{
    /// <summary>
    /// String data type.
    /// </summary>
    public static readonly AttributeDataType String = new AttributeDataType("String", 1);
    /// <summary>
    /// Number data type.
    /// </summary>
    public static readonly AttributeDataType Number = new AttributeDataType("Number", 2);
    /// <summary>
    /// Boolean data type.
    /// </summary>
    public static readonly AttributeDataType Boolean = new AttributeDataType("Boolean", 3);
    /// <summary>
    /// Date data type.
    /// </summary>
    public static readonly AttributeDataType Date = new AttributeDataType("Date", 4);
    /// <summary>
    /// Enum data type.
    /// </summary>
    public static readonly AttributeDataType Enum = new AttributeDataType("Enum", 5);
    /// <summary>
    /// Json data type.
    /// </summary>
    public static readonly AttributeDataType Json = new AttributeDataType("Json", 6);
    /// <summary>
    /// File data type.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    private AttributeDataType(string name, int value) : base(name, value) { }
}
