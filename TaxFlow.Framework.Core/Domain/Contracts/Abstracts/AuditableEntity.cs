namespace Core.Domain.Contracts.Abstracts;

/// <summary>
/// Base class for entities that have audit information (creation and modification metadata).
/// </summary>
public abstract class AuditableEntity : BaseEntity<Guid>, IAuditable
{
    /// <summary>
    /// Protected constructor that initializes the entity identifier.
    /// </summary>
    protected AuditableEntity()
    {
        Id = Guid.CreateVersion7();
    }

    /// <summary>
    /// Gets the creation date/time of the entity.
    /// </summary>
    public DateTimeOffset Created { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who created the entity.
    /// </summary>
    public Guid CreatedBy { get; private set; }

    /// <summary>
    /// Gets the date/time of the last modification.
    /// </summary>
    public DateTimeOffset LastModified { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who last modified the entity, if any.
    /// </summary>
    public Guid? LastModifiedBy { get; private set; }
}
