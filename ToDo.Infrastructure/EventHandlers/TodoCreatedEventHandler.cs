using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Domain.Events;

namespace TodoApp.Infrastructure.EventHandlers;

public class TodoCreatedEventHandler : INotificationHandler<TodoCreatedEvent>
{
    private readonly ILogger<TodoCreatedEventHandler> _logger;

    public TodoCreatedEventHandler(ILogger<TodoCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event Handled: Todo Created - {Title} (ID: {Id})",
            notification.Item.Title, notification.Item.Id);

        return Task.CompletedTask;
    }
}