using System.Collections.ObjectModel;
using Core.Domain.Contracts;
using Core.Domain.Enums;

namespace Core.Domain.Contracts.Abstracts;

public abstract class AuditableEntityWithExtendableAttribute : SoftAuditableEntity
{
    // Holds extendable attributes for the entity
    public Collection<ExtendedAttribute> Attributes { get; } = new();

    public ExtendedAttribute AddAttribute(string key, string value, AttributeDataType dataType, bool isRequired = false)
    {
        var attr = new ExtendedAttribute
        {
            Key = key,
            Value = value,
            DataType = dataType,
            IsRequired = isRequired,
            ValidFrom = DateTimeOffset.UtcNow
        };
        if (!Attributes.Contains(attr))
            Attributes.Add(attr);
        return attr;
    }

    public bool RemoveAttribute(ExtendedAttribute attribute)
    {
        return Attributes.Remove(attribute);
    }
}
