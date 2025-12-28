using MediatR;

namespace ToDo.Application.Todos.Commands;

public record DeleteTodoCommand(string Id) : IRequest<bool>;