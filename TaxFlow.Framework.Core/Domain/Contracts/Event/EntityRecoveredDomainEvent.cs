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
        /// <summary>
        /// Gets the UTC date and time when this domain event occurred.
        /// </summary>
        /// <remarks>
        /// This timestamp is automatically set to the current UTC time when the event is created.
        /// </remarks>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
    /// <summary>
    /// Represents a domain event that occurs when an existing entity is updated.
    /// </summary>
    /// <typeparam name="TId">The type of the unique identifier for the updated entity.</typeparam>
    /// <param name="Id">The unique identifier of the entity that was updated.</param>
    /// <param name="Author">The unique identifier of the user or process that updated the entity.</param>
    public record EntityUpdatedDomainEvent<TId>(TId Id, Guid Author) : IDomainEvent
    {
        /// <summary>
        /// Gets the UTC date and time when this domain event occurred.
        /// </summary>
        /// <remarks>
        /// This timestamp is automatically set to the current UTC time when the event is created.
        /// </remarks>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
    /// <summary>
    /// Represents a domain event that occurs when a soft-deleted entity is recovered.
    /// </summary>
    /// <typeparam name="TId">The type of the unique identifier for the recovered entity.</typeparam>
    /// <param name="Id">The unique identifier of the entity that was recovered from soft deletion.</param>
    /// <param name="Author">The unique identifier of the user or process that performed the recovery operation.</param>
    public record EntityRecoveredDomainEvent<TId>(TId Id, Guid Author) : IDomainEvent
    {
        /// <summary>
        /// Gets the UTC date and time when this domain event occurred.
        /// </summary>
        /// <remarks>
        /// This timestamp is automatically set to the current UTC time when the event is created.
        /// </remarks>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
    /// <summary>
    /// Represents a domain event that occurs when an entity is soft-deleted.
    /// </summary>
    /// <typeparam name="TId">The type of the unique identifier for the deleted entity.</typeparam>
    /// <param name="Id">The unique identifier of the entity that was soft-deleted.</param>
    /// <param name="Author">The unique identifier of the user or process that performed the soft deletion.</param>
    public record EntityDeletedDomainEvent<TId>(TId Id, Guid Author) : IDomainEvent
    {
        /// <summary>
        /// Gets the UTC date and time when this domain event occurred.
        /// </summary>
        /// <remarks>
        /// This timestamp is automatically set to the current UTC time when the event is created.
        /// </remarks>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
