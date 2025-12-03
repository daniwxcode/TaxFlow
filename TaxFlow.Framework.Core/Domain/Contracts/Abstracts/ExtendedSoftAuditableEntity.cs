using System.Collections.ObjectModel;

using Core.Domain.Contracts;
using Core.Domain.Enums;

namespace Core.Domain.Contracts.Abstracts;

public abstract class ExtendedSoftAuditableEntity : SoftAuditableEntity
{
    // Holds extendable attributes for the entity
    private List<ExtendedAttribute> _attributes = new();
    public IReadOnlyCollection<ExtendedAttribute> Attributes => _attributes.AsReadOnly();
    public ExtendedAttribute? GetAttribute(string key, DateTimeOffset? date = null)
    {
        date ??= DateTimeOffset.UtcNow;
        return Attributes.FirstOrDefault(a => a.Key == key && a.ValidFrom <= date && (a.ValidTo == null || a.ValidTo >= date));
    }

    public ExtendedAttribute AddOrUpdateAttribute(string key, string value, AttributeDataType dataType, bool isRequired = false)
    {
        var existing = GetAttribute(key);
        if (existing != null)
        {
            existing.Value = value;
            existing.DataType = dataType;
            existing.IsRequired = isRequired;
            existing.ValidFrom = DateTimeOffset.UtcNow;
            existing.ValidTo = null;
            return existing;
        }
        return AddAttribute(key, value, dataType, isRequired);
    }
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
