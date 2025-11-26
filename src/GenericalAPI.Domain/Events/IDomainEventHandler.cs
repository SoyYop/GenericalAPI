namespace GenericalAPI.Domain.Events
{
    // Handles a domain event; implement per event type in the domain layer.
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
