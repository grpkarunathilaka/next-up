using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    Task<IEnumerable<Todo>> GetAllTodosAsync();
    Task<Todo?> GetTodoByIdAsync(string id);
    Task<Todo> CreateTodoAsync(CreateTodoDto createDto);
    Task<Todo?> UpdateTodoAsync(string id, UpdateTodoDto updateDto);
    Task<bool> DeleteTodoAsync(string id);
    Task<TodoStatistics> GetStatisticsAsync();
    Task<IEnumerable<Todo>> SearchTodosAsync(string query);
    Task<bool> ReorderTodosAsync(List<string> orderedIds);
}