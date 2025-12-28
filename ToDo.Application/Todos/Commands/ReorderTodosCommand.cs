using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Domain.Interfaces;

namespace ToDo.Application.Todos.Commands;

    public record ReorderTodosCommand(List<string> OrderedIds) : IRequest;

public class ReorderTodosCommandHandler : IRequestHandler<ReorderTodosCommand>
{
    private readonly ITodoRepository _repository;

    public ReorderTodosCommandHandler(ITodoRepository repository) => _repository = repository;

    public async Task Handle(ReorderTodosCommand request, CancellationToken ct)
    {
        await _repository.ReorderAsync(request.OrderedIds);
    }
}