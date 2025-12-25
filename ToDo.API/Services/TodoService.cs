using Microsoft.AspNetCore.SignalR;
using TodoApi.Hubs;
using TodoApi.Models;
using TodoApi.Repositories;
using System.Text.Json;

namespace TodoApi.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;
    private readonly ILogger<TodoService> _logger;
    private readonly ICacheService _cache;
    private readonly IHubContext<TodoHub> _hubContext;
    private const string CacheKey = "all_todos";

    public TodoService(
        ITodoRepository repository,
        ILogger<TodoService> logger,
        ICacheService cache,
        IHubContext<TodoHub> hubContext)
    {
        _repository = repository;
        _logger = logger;
        _cache = cache;
        _hubContext = hubContext;
    }

    public async Task<IEnumerable<Todo>> GetAllTodosAsync()
    {
        _logger.LogInformation("Retrieving all todos");

        var cached = _cache.Get<IEnumerable<Todo>>(CacheKey);
        if (cached != null)
        {
            _logger.LogInformation("Returning cached todos");
            return cached;
        }

        var todos = await _repository.GetAllAsync();
        _cache.Set(CacheKey, todos, TimeSpan.FromMinutes(5));
        return todos;
    }

    public async Task<Todo?> GetTodoByIdAsync(string id)
    {
        _logger.LogInformation("Retrieving todo with id: {TodoId}", id);
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Todo> CreateTodoAsync(CreateTodoDto createDto)
    {
        _logger.LogInformation("Creating new todo: {Title}", createDto.Title);

        var todo = new Todo
        {
            Title = createDto.Title.Trim(),
            IsCompleted = false,
            Priority = createDto.Priority,
            Category = createDto.Category,
            DueDate = createDto.DueDate,
            Tags = createDto.Tags ?? new List<string>(),
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(todo);
        _cache.Remove(CacheKey);

        // Notify connected clients via SignalR
        await _hubContext.Clients.All.SendAsync("TodoCreated", JsonSerializer.Serialize(created));

        return created;
    }

    public async Task<Todo?> UpdateTodoAsync(string id, UpdateTodoDto updateDto)
    {
        _logger.LogInformation("Updating todo with id: {TodoId}", id);

        if (updateDto.Title != null)
            updateDto.Title = updateDto.Title.Trim();

        var updated = await _repository.UpdateAsync(id, updateDto);
        if (updated != null)
        {
            _cache.Remove(CacheKey);
            await _hubContext.Clients.All.SendAsync("TodoUpdated", JsonSerializer.Serialize(updated));
        }

        return updated;
    }

    public async Task<bool> DeleteTodoAsync(string id)
    {
        _logger.LogInformation("Deleting todo with id: {TodoId}", id);
        var deleted = await _repository.DeleteAsync(id);

        if (deleted)
        {
            _cache.Remove(CacheKey);
            await _hubContext.Clients.All.SendAsync("TodoDeleted", id);
        }

        return deleted;
    }

    public async Task<TodoStatistics> GetStatisticsAsync()
    {
        var todos = await GetAllTodosAsync();
        var todoList = todos.ToList();
        var today = DateTime.Today;

        var stats = new TodoStatistics
        {
            Total = todoList.Count,
            Completed = todoList.Count(t => t.IsCompleted),
            Pending = todoList.Count(t => !t.IsCompleted),
            CompletionRate = todoList.Count > 0
                ? Math.Round((double)todoList.Count(t => t.IsCompleted) / todoList.Count * 100, 2)
                : 0,
            ByPriority = todoList.GroupBy(t => t.Priority)
                .ToDictionary(g => g.Key, g => g.Count()),
            ByCategory = todoList.Where(t => t.Category != null)
                .GroupBy(t => t.Category!)
                .ToDictionary(g => g.Key, g => g.Count()),
            DueToday = todoList.Count(t => t.DueDate?.Date == today),
            Overdue = todoList.Count(t => t.DueDate < today && !t.IsCompleted)
        };

        return stats;
    }

    public async Task<IEnumerable<Todo>> SearchTodosAsync(string query)
    {
        var todos = await GetAllTodosAsync();
        var lowerQuery = query.ToLower();

        return todos.Where(t =>
            t.Title.ToLower().Contains(lowerQuery) ||
            (t.Category?.ToLower().Contains(lowerQuery) ?? false) ||
            t.Tags.Any(tag => tag.ToLower().Contains(lowerQuery))
        );
    }

    public async Task<bool> ReorderTodosAsync(List<string> orderedIds)
    {
        await _repository.ReorderAsync(orderedIds);
        _cache.Remove(CacheKey);
        return true;
    }
}