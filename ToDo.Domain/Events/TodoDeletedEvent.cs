namespace TodoApp.Domain.Events;

using TodoApp.Domain.Common;
using TodoApp.Domain.Entities;


public class TodoDeletedEvent : BaseEvent
{
    public string TodoId { get; }
    public TodoDeletedEvent(string id) => TodoId = id;
}