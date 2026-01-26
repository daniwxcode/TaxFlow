namespace Core.Domain.Contracts.Event
{
    /// <summary>
    /// Represents a domain event that occurs when a new entity is created.
    /// </summary>
    /// <typeparam name="TId">The type of the unique identifier for the created entity.</typeparam>
    /// <param name="Id">The unique identifier of the entity that was created.</param>
    /// <param name="Author">The unique identifier of the user or process that created the entity.</param>
    public record EntityCreatedDomainEvent<TId>(TId Id, Guid Author) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
    /// <summary>
    /// Represents a domain event that occurs when an existing entity is updated.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <param name="Id"></param>
    /// <param name="Author"></param>
    public record EntityUpdatedDomainEvent<TId>(TId Id, Guid Author) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
    /// <summary>
    /// Represents a domain event that occurs when a soft-deleted entity is recovered.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <param name="Id"></param>
    /// <param name="Author"></param>
    public record EntityRecoveredDomainEvent<TId>(TId Id, Guid Author) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
    /// <summary>
    /// Represents a domain event that occurs when an entity is soft-deleted.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <param name="Id"></param>
    /// <param name="Author"></param>
    public record EntityDeletedDomainEvent<TId>(TId Id, Guid Author) : IDomainEvent
    {
        /// <summary>
        /// Represents the date and time when the event occurred.
        /// </summary>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
