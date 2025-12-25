using System.Collections.Concurrent;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<string, Todo> _todos = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ILogger<InMemoryTodoRepository> _logger;

    public InMemoryTodoRepository(ILogger<InMemoryTodoRepository> logger)
    {
        _logger = logger;
        SeedData();
    }

    private void SeedData()
    {
        // Add some sample data
        var sampleTodos = new[]
        {
            new Todo { Title = "Welcome to Advanced Todo API", Priority = "high", Category = "Getting Started", Tags = new() { "welcome", "tutorial" } },
            new Todo { Title = "Try drag and drop to reorder", Priority = "medium", Category = "Features" },
            new Todo { Title = "Press Ctrl+Z to undo changes", Priority = "low", Category = "Features" }
        };

        foreach (var todo in sampleTodos)
        {
            _todos[todo.Id] = todo;
        }
    }

    public Task<IEnumerable<Todo>> GetAllAsync()
    {
        return Task.FromResult(_todos.Values.OrderBy(t => t.Order).ThenBy(t => t.CreatedAt).AsEnumerable());
    }

    public Task<Todo?> GetByIdAsync(string id)
    {
        _todos.TryGetValue(id, out var todo);
        return Task.FromResult(todo);
    }

    public async Task<Todo> CreateAsync(Todo todo)
    {
        await _semaphore.WaitAsync();
        try
        {
            todo.Order = _todos.Count;
            _todos[todo.Id] = todo;
            _logger.LogInformation("Created todo {TodoId}: {Title}", todo.Id, todo.Title);
            return todo;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Todo?> UpdateAsync(string id, UpdateTodoDto updateDto)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (!_todos.TryGetValue(id, out var todo))
                return null;

            if (updateDto.Title != null) todo.Title = updateDto.Title;
            if (updateDto.IsCompleted.HasValue) todo.IsCompleted = updateDto.IsCompleted.Value;
            todo.Priority = updateDto.Priority;
            if (updateDto.Category != null) todo.Category = updateDto.Category;
            if (updateDto.DueDate.HasValue) todo.DueDate = updateDto.DueDate;
            if (updateDto.Tags != null) todo.Tags = updateDto.Tags;

            todo.UpdatedAt = DateTime.UtcNow;
            _todos[id] = todo;

            _logger.LogInformation("Updated todo {TodoId}", id);
            return todo;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task<bool> DeleteAsync(string id)
    {
        var result = _todos.TryRemove(id, out _);
        if (result)
        {
            _logger.LogInformation("Deleted todo {TodoId}", id);
        }
        return Task.FromResult(result);
    }

    public async Task ReorderAsync(List<string> orderedIds)
    {
        await _semaphore.WaitAsync();
        try
        {
            for (int i = 0; i < orderedIds.Count; i++)
            {
                if (_todos.TryGetValue(orderedIds[i], out var todo))
                {
                    todo.Order = i;
                }
            }
            _logger.LogInformation("Reordered {Count} todos", orderedIds.Count);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}