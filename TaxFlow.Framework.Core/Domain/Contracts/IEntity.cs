using Core.Domain.Contracts.Event;

using System.Collections.ObjectModel;

namespace Core.Domain.Contracts;
public interface IEntity
{
    Collection<IDomainEvent> DomainEvents { get; }
}

public interface IEntity<out TId> : IEntity
{
    TId Id { get; }
}
