using Core.Domain.Contracts.Event;

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Contracts.Abstracts;

/// <summary>
/// Base class for domain entities providing a typed identifier and domain event collection.
/// </summary>
/// <typeparam name="TId">Type of the entity identifier.</typeparam>
public abstract class BaseEntity<TId> : IEntity<TId>
{
    /// <summary>
    /// Entity identifier. Protected setter allows derived entities to initialize the Id while keeping it immutable from consumers.
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// Collection of domain events raised by this entity. Not mapped by ORMs.
    /// </summary>
    [NotMapped]
    public Collection<IDomainEvent> DomainEvents { get; } = new Collection<IDomainEvent>();

    /// <summary>
    /// Queue a domain event for this entity if it is not already present in the collection.
    /// </summary>
    /// <param name="@event">The domain event to queue.</param>
    public void QueueDomainEvent(IDomainEvent @event)
    {
        if (!DomainEvents.Contains(@event))
            DomainEvents.Add(@event);
    }
}
