using Core.Domain.Contracts.Event;

using System.Collections.ObjectModel;

namespace Core.Domain.Contracts;
/// <summary>
///    Base interface for all entities in the domain.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    Collection<IDomainEvent> DomainEvents { get; }
}
/// <summary>
/// Interface defining an entity with a typed unique identifier.
/// </summary>
/// <typeparam name="TId"></typeparam>
public interface IEntity<out TId> : IEntity
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    TId Id { get; }
}
