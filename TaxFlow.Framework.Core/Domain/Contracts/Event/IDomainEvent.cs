namespace Core.Domain.Contracts.Event
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
