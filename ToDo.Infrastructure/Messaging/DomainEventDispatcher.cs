using MediatR;
using TodoApp.Domain.Common;

namespace TodoApp.Infrastructure.Messaging;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IEnumerable<BaseEntity> entities);
}

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public DomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchEventsAsync(IEnumerable<BaseEntity> entities)
    {
        foreach (var entity in entities)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                await _publisher.Publish(domainEvent);
            }
        }
    }
}