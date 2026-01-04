using AutoMapper;
using MediatR;
using TodoApp.Domain.Interfaces;

namespace ToDo.Application.Todos.Queries;

public record GetTodoByIdQuery(string Id) : IRequest<TodoDto?>;

public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, TodoDto?>
{
    private readonly ITodoRepository _repository;
    private readonly IMapper _mapper;

    public GetTodoByIdQueryHandler(ITodoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TodoDto?> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var todoItem = await _repository.GetByIdAsync(request.Id);

        if (todoItem == null)
        {
            return null; // Todo item not found
        }

        return _mapper.Map<TodoDto>(todoItem);
    }
}
