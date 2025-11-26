using GenericalAPI.Domain.Events;

namespace GenericalAPI.Application.Abstractions.Messaging
{
    // Publishes domain/integration events to the chosen bus.
    public interface IEventPublisher
    {
        Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
