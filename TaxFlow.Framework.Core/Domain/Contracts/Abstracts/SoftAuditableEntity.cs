using Core.Domain.Contracts.Event;

namespace Core.Domain.Contracts.Abstracts;

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
