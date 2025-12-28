using MediatR;
using TodoApp.Domain.Interfaces;
using TodoApp.Domain.Entities; // Assuming TodoItem is in this namespace

namespace ToDo.Application.Todos.Commands;

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, bool>
{
    private readonly ITodoRepository _todoRepository;

    public DeleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<bool> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await _todoRepository.GetByIdAsync(request.Id);

        if (todoItem == null)
        {
            return false; // Todo item not found
        }

        await _todoRepository.DeleteAsync(todoItem);
        return true;
    }
}
