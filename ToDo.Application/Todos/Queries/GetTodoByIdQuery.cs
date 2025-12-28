using MediatR;

namespace ToDo.Application.Todos.Queries;

public record GetTodoByIdQuery(string Id) : IRequest<TodoDto?>;
