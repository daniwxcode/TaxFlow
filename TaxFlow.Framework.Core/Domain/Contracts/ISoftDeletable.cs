namespace Core.Domain.Contracts;
public interface ISoftDeletable: IAuditable
{
    DateTimeOffset? Deleted { get; }
    Guid? DeletedBy { get; }
    DateTimeOffset? LastDeletedOn { get; }
    Guid? LastDeletedby { get; }
    DateTimeOffset? LastRecovered { get; }

    Guid? LastRecoveredBy { get; }
    void Recover(Guid recoveredBy);
}
