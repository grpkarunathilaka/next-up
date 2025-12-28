using System.Collections.Concurrent;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Infrastructure.Persistence;

public class InMemoryTodoRepository : ITodoRepository
{
    // Thread-safe in-memory storage
    private static readonly ConcurrentDictionary<string, TodoItem> _todos = new();

    public async Task<TodoItem?> GetByIdAsync(string id)
    {
        _todos.TryGetValue(id, out var todo);
        return await Task.FromResult(todo);
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        return await Task.FromResult(_todos.Values.OrderBy(t => t.Position));
    }

    public async Task<IEnumerable<TodoItem>> SearchAsync(string query)
    {
        var results = _todos.Values
            .Where(t => t.Title.Contains(query, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(results);
    }

    public async Task AddAsync(TodoItem todo)
    {
        _todos.TryAdd(todo.Id, todo);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(TodoItem todo)
    {
        _todos[todo.Id] = todo;
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(TodoItem todo)
    {
        _todos.TryRemove(todo.Id, out _);
        await Task.CompletedTask;
    }

    public async Task ReorderAsync(List<string> orderedIds)
    {
        for (int i = 0; i < orderedIds.Count; i++)
        {
            if (_todos.TryGetValue(orderedIds[i], out var todo))
            {
                todo.SetPosition(i);
            }
        }
        await Task.CompletedTask;
    }

    public async Task<int> GetTotalCountAsync() => await Task.FromResult(_todos.Count);

    public async Task<int> GetCompletedCountAsync() =>
        await Task.FromResult(_todos.Values.Count(t => t.IsCompleted));
}