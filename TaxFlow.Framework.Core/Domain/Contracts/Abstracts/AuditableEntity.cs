using Core.Domain.Contracts.Event;

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Contracts.Abstracts;


public abstract class BaseEntity<TId> : IEntity<TId>
{
    public TId Id { get; protected init; } = default!;
    [NotMapped]
    public Collection<IDomainEvent> DomainEvents { get; } = new Collection<IDomainEvent>();
    public void QueueDomainEvent(IDomainEvent @event)
    {
        if (!DomainEvents.Contains(@event))
            DomainEvents.Add(@event);
    }
}
public abstract class AuditableEntity : BaseEntity<Guid>, IAuditable
{
    public DateTimeOffset Created { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset LastModified { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
}
public abstract class SoftAuditableEntity: AuditableEntity, ISoftDeletable
{
    public DateTimeOffset? Deleted { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public DateTimeOffset? LastDeletedOn { get; private set; }
    public Guid? LastDeletedby { get; private set; }
    public DateTimeOffset? LastRecovered { get; private set; }
    public Guid? LastRecoveredBy { get; private set; }
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
