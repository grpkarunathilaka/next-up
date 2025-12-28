namespace TodoApp.Domain.Entities;

using TodoApp.Domain.Common;
using TodoApp.Domain.Events;

public class TodoItem : BaseEntity
{
    public string Title { get; private set; }
    public bool IsCompleted { get; private set; }
    public int Position { get; private set; } // For the Reorder functionality
    public string Priority { get; private set; }

    public TodoItem(string title, int position = 0, string priority = "medium")
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.");

        Title = title;
        Position = position;
        Priority = priority;

        // Record that a new todo was created
        AddDomainEvent(new TodoCreatedEvent(this));
    }

    public void UpdateDetails(string? title, bool? isCompleted, string? priority)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            Title = title;
        }
        if (isCompleted.HasValue)
        {
            IsCompleted = isCompleted.Value;
        }
        if (!string.IsNullOrWhiteSpace(priority))
        {
            Priority = priority;
        }
    }

    public void SetPosition(int newPosition)
    {
        Position = newPosition;
    }
}
