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
