namespace TodoApp.Domain.Interfaces;

using TodoApp.Domain.Entities;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(string id);
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<IEnumerable<TodoItem>> SearchAsync(string query);
    Task AddAsync(TodoItem todo);
    Task UpdateAsync(TodoItem todo);
    Task DeleteAsync(TodoItem todo);
    Task ReorderAsync(List<string> orderedIds);
    Task<int> GetTotalCountAsync();
    Task<int> GetCompletedCountAsync();
}