namespace Core.Domain.Contracts.Abstracts;

/// <summary>
/// Represents an auditable entity that tracks a temporal validity period, including start and optional end dates.
/// </summary>
/// <remarks>Use this base class for entities that require auditing and validity intervals, such as records that
/// are only valid within a specific time range. The validity period is defined by the <see cref="ValidFrom"/> and <see
/// cref="ValidTo"/> properties. Entities inheriting from this class can be used to model time-bound data, such as
/// historical records or scheduled events.</remarks>
public abstract class TemporalAuditableEntity : AuditableEntity, ITemporalValiditable
{
    /// <summary>
    /// Gets or sets the start date/time of the entity's validity period.
    /// </summary>
    public DateTimeOffset ValidFrom { get; set; }
    /// <summary>
    /// Gets or sets the end date/time of the entity's validity period, if any.
    /// </summary>
    public DateTimeOffset? ValidTo { get; set; }
}