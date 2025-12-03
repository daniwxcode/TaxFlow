using Core.Domain.Contracts.Event;

namespace Core.Domain.Contracts.Abstracts;

/// <summary>
/// Base class for auditable entities that support soft-delete semantics and recovery.
/// </summary>
public abstract class SoftAuditableEntity: AuditableEntity, ISoftDeletable
{
    /// <summary>
    /// Date/time when the entity was soft-deleted, if any.
    /// </summary>
    public DateTimeOffset? Deleted { get; private set; }

    /// <summary>
    /// Identifier of the user who deleted the entity, if any.
    /// </summary>
    public Guid? DeletedBy { get; private set; }

    /// <summary>
    /// The previous deletion timestamp before the last recovery.
    /// </summary>
    public DateTimeOffset? LastDeletedOn { get; private set; }

    /// <summary>
    /// The previous deleter identifier before the last recovery.
    /// </summary>
    public Guid? LastDeletedby { get; private set; }

    /// <summary>
    /// The timestamp when the entity was last recovered from soft-delete, if any.
    /// </summary>
    public DateTimeOffset? LastRecovered { get; private set; }

    /// <summary>
    /// Identifier of the user who last recovered the entity, if any.
    /// </summary>
    public Guid? LastRecoveredBy { get; private set; }

    /// <summary>
    /// Recover the entity from soft-delete state and queue an <see cref="EntityRecoveredDomainEvent{TId}"/>.
    /// </summary>
    /// <param name="recoveredBy">Identifier of the user performing the recovery.</param>
    public void Recover(Guid recoveredBy)
    {       
        LastDeletedOn = Deleted;
        LastDeletedby = DeletedBy;        
        LastRecovered = DateTimeOffset.UtcNow;
        LastRecoveredBy = recoveredBy;
        // Clear soft-delete markers; audit fields updated by infrastructure
        Deleted = null;
        DeletedBy = null;
        QueueDomainEvent(new EntityRecoveredDomainEvent<Guid>(Id, recoveredBy));
    }


}
