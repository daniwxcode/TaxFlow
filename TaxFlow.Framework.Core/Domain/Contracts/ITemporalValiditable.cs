namespace Core.Domain.Contracts;
/// <summary>
/// Interface defining temporal validity for entities.
/// </summary>
public interface ITemporalValiditable
{
    /// <summary>
    /// Gets or sets the date/time from which the entity is considered valid.
    /// </summary>
    DateTimeOffset ValidFrom { get; set; }
    /// <summary>
    /// Gets or sets the date/time until which the entity is considered valid.
    /// </summary>
    DateTimeOffset? ValidTo { get; set; }
    /// <summary>
    /// Gets a value indicating whether the entity is currently valid.
    /// </summary>
    bool IsValid => ValidFrom <= DateTimeOffset.UtcNow && (ValidTo == null || ValidTo > DateTimeOffset.UtcNow);
}
