namespace Core.Domain.Contracts.Abstracts;

public abstract class ExtendedTemporalSoftAuditableEntity : ExtendedSoftAuditableEntity, ITemporalValiditable
{
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }
}