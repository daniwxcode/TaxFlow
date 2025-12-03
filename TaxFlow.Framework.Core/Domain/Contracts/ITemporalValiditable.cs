namespace Core.Domain.Contracts;

public interface ITemporalValiditable
{
    DateTimeOffset ValidFrom { get; set; }
    DateTimeOffset? ValidTo { get; set; }
    bool IsValid => ValidFrom <= DateTimeOffset.UtcNow && (ValidTo == null || ValidTo > DateTimeOffset.UtcNow);
}
