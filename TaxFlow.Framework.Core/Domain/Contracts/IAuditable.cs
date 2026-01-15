namespace Core.Domain.Contracts;
/// <summary>
///     Interface defining audit information for entities.
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// Gets the creation date/time of the entity.
    /// </summary>
    DateTimeOffset Created { get; }
    /// <summary>
    ///  Gets the identifier of the user who created the entity.
    /// </summary>
    Guid CreatedBy { get; }
    /// <summary>
    ///  Gets the date/time of the last modification.
    /// </summary>
    DateTimeOffset LastModified { get; }
    /// <summary>
    ///  Gets the identifier of the user who last modified the entity, if any.
    /// </summary>
    Guid? LastModifiedBy { get; }
}
