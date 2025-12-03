using Core.Domain.Contracts.Event;

namespace Core.Domain.Tax.Event
{
    internal class AssetTypeRenamedDomainEvent(Guid Id,string NewName) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}