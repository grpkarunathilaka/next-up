

using AutoMapper;
using MediatR;
using ToDo.Application.Todos.Queries;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;

namespace ToDo.Application.Todos.Commands;
public record CreateTodoCommand(string Title, string Priority) : IRequest<TodoDto>;

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, TodoDto>
{
    private readonly ITodoRepository _repository;
    private readonly IMapper _mapper;

    public CreateTodoCommandHandler(ITodoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TodoDto> Handle(CreateTodoCommand request, CancellationToken ct)
    {
        var entity = new TodoItem(request.Title, priority: request.Priority);
        await _repository.AddAsync(entity);
        return _mapper.Map<TodoDto>(entity);
    }
}