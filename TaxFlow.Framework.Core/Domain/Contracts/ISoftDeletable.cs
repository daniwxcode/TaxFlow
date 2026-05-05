namespace Core.Domain.Contracts;
/// <summary>
/// Interface defining soft deletion capabilities for entities.
/// </summary>
public interface ISoftDeletable : IAuditable
{
    /// <summary>
    /// Gets the date/time when the entity was deleted, if applicable.
    /// </summary>
    DateTimeOffset? Deleted { get; }
    /// <summary>
    /// Gets the identifier of the user who deleted the entity, if applicable.
    /// </summary>
    Guid? DeletedBy { get; }
    /// <summary>
    /// Gets the date/time when the entity was last soft-deleted.
    /// </summary>
    DateTimeOffset? LastDeletedOn { get; }
    /// <summary>
    /// Gets the identifier of the user who last soft-deleted the entity.
    /// </summary>
    Guid? LastDeletedby { get; }
    /// <summary>
    /// Gets the date/time when the entity was last recovered.
    /// </summary>
    DateTimeOffset? LastRecovered { get; }

    /// <summary>
    /// Gets the identifier of the user who last recovered the entity.
    /// </summary>
    Guid? LastRecoveredBy { get; }
    /// <summary>
    /// Marks the entity as recovered.
    /// </summary>
    /// <param name="recoveredBy">The identifier of the user performing the recovery.</param>
    void Recover(Guid recoveredBy);
}
