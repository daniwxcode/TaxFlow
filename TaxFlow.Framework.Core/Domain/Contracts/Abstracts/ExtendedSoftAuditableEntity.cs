using System.Collections.ObjectModel;
using System.Linq;

using Core.Domain.Contracts;
using Core.Domain.Enums;

namespace Core.Domain.Contracts.Abstracts;

/// <summary>
/// Extended auditable entity that supports a collection of extendable attributes with temporal validity.
/// </summary>
public abstract class ExtendedSoftAuditableEntity : SoftAuditableEntity
{
    /// <summary>
    /// Backing list of extended attributes. Use <see cref="Attributes"/> to access a read-only view.
    /// </summary>
    protected List<ExtendedAttribute> _attributes = new();

    /// <summary>
    /// Read-only collection of extended attributes attached to this entity.
    /// </summary>
    public IReadOnlyCollection<ExtendedAttribute> Attributes => _attributes.AsReadOnly();

    /// <summary>
    /// Gets the attribute with the specified key that is valid for the given date/time.
    /// If <paramref name="date"/> is null, the current UTC time is used.
    /// </summary>
    /// <param name="key">Attribute key to search for.</param>
    /// <param name="date">Optional date to test temporal validity.</param>
    /// <returns>The matching <see cref="ExtendedAttribute"/>, or null if none found.</returns>
    public ExtendedAttribute? GetAttribute(string key, DateTimeOffset? date = null)
    {
        date ??= DateTimeOffset.UtcNow;
        return Attributes.FirstOrDefault(a => a.Key == key && a.ValidFrom <= date && (a.ValidTo == null || a.ValidTo >= date));
    }

    /// <summary>
    /// Adds a new attribute or updates an existing one identified by key.
    /// </summary>
    /// <param name="key">Attribute key.</param>
    /// <param name="value">Attribute value.</param>
    /// <param name="dataType">Attribute data type.</param>
    /// <param name="isRequired">Whether the attribute is required.</param>
    /// <returns>The added or updated <see cref="ExtendedAttribute"/> instance.</returns>
    public ExtendedAttribute AddOrUpdateAttribute(string key, string value, AttributeDataType dataType, bool isRequired = false)
    {
        var existing = GetAttribute(key);
        if (existing != null)
        {
            existing.UpdateValue(value, dataType, isRequired);
            return existing;
        }
        return AddAttribute(key, value, dataType, isRequired);
    }

    /// <summary>
    /// Adds a new extended attribute with the provided parameters.
    /// </summary>
    /// <param name="key">Attribute key.</param>
    /// <param name="value">Attribute value.</param>
    /// <param name="dataType">Attribute data type.</param>
    /// <param name="isRequired">Whether the attribute is required.</param>
    /// <returns>The newly created <see cref="ExtendedAttribute"/>.</returns>
    public ExtendedAttribute AddAttribute(string key, string value, AttributeDataType dataType, bool isRequired = false)
    {
        var attr = ExtendedAttribute.Create(key, value, dataType, isRequired, DateTimeOffset.UtcNow, null);
        // Prevent duplicates by key
        if (!_attributes.Any(a => a.Key.Equals(attr.Key, StringComparison.OrdinalIgnoreCase)))
            _attributes.Add(attr);
        return attr;
    }

    /// <summary>
    /// Removes the provided attribute instance from the entity.
    /// </summary>
    /// <param name="attribute">Attribute instance to remove.</param>
    /// <returns>True if removed; otherwise false.</returns>
    public bool RemoveAttribute(ExtendedAttribute attribute)
    {
        return _attributes.Remove(attribute);
    }
}
