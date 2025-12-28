namespace TodoApp.Domain.Events;

using TodoApp.Domain.Common;
using TodoApp.Domain.Entities;

public class TodoCreatedEvent : BaseEvent
{
    public TodoItem Item { get; }
    public TodoCreatedEvent(TodoItem item) => Item = item;
}