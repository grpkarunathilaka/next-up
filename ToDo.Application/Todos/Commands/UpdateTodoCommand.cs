using AutoMapper;
using MediatR;
using ToDo.Application.Todos.Queries;
using TodoApp.Domain.Interfaces;

namespace ToDo.Application.Todos.Commands;
public class UpdateTodoCommand : IRequest<TodoDto?>
{
    public string Id { get; set; } = string.Empty;
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
    public string? Priority { get; set; }
}

public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, TodoDto?>
{
    private readonly ITodoRepository _repository;
    private readonly IMapper _mapper;

    public UpdateTodoCommandHandler(ITodoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TodoDto?> Handle(UpdateTodoCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null) return null;

        entity.UpdateDetails(request.Title, request.IsCompleted, request.Priority);
        await _repository.UpdateAsync(entity);

        return _mapper.Map<TodoDto>(entity);
    }
}
