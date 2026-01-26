namespace Core.Domain.Contracts.Event;
/// <summary>
/// Represents a domain event that signifies a significant occurrence within the domain.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Represents the date and time when the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }
}
