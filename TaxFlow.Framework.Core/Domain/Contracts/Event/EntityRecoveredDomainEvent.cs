namespace Core.Domain.Contracts.Event
{
    public record EntityCreatedDomainEvent<TId>(TId Id, Guid Author) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
    public record EntityUpdatedDomainEvent<TId>(TId Id, Guid Author) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
    public record EntityRecoveredDomainEvent<TId>(TId Id, Guid Author) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
    public record EntityDeletedDomainEvent<TId>(TId Id, Guid Author) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}