namespace TodoApi.Repositories;

using TodoApi.Models;

public interface ITodoRepository
{
    Task<IEnumerable<Todo>> GetAllAsync();
    Task<Todo?> GetByIdAsync(string id);
    Task<Todo> CreateAsync(Todo todo);
    Task<Todo?> UpdateAsync(string id, UpdateTodoDto updateDto);
    Task<bool> DeleteAsync(string id);
    Task ReorderAsync(List<string> orderedIds);
}
