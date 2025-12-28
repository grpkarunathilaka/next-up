namespace TodoApp.Domain.Events;

using TodoApp.Domain.Common;
using TodoApp.Domain.Entities;

public class TodosReorderedEvent : BaseEvent
{
    public List<string> OrderedIds { get; }
    public TodosReorderedEvent(List<string> ids) => OrderedIds = ids;
}