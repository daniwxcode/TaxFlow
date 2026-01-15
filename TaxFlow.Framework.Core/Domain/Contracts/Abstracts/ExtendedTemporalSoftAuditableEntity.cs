namespace Core.Domain.Contracts.Abstracts;

/// <summary>
/// Extended auditable entity that supports a collection of extendable attributes with temporal validity.
/// </summary>
public abstract class ExtendedTemporalSoftAuditableEntity : ExtendedSoftAuditableEntity, ITemporalValiditable
{
    /// <summary>
    /// The start of the validity period for this entity.
    /// </summary>
    public DateTimeOffset ValidFrom { get; set; }
    /// <summary>
    /// The end of the validity period for this entity, if any.
    /// </summary>
    public DateTimeOffset? ValidTo { get; set; }
}