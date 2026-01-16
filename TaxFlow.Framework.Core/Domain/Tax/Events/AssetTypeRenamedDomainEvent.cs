using Core.Domain.Contracts.Event;

namespace Core.Domain.Tax.Events;

/// <summary>
/// Domain event representing the renaming of an asset type.
/// </summary>
/// <param name="Id"></param>
/// <param name="NewName"></param>
internal record AssetTypeRenamedDomainEvent(Guid Id, string NewName) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
