
using AutoMapper;
using MediatR;
using TodoApp.Domain.Interfaces;

namespace ToDo.Application.Todos.Queries;

public record GetAllTodosQuery() : IRequest<IEnumerable<TodoDto>>;

public class GetAllTodosQueryHandler : IRequestHandler<GetAllTodosQuery, IEnumerable<TodoDto>>
{
    private readonly ITodoRepository _repository;
    private readonly IMapper _mapper;

    public GetAllTodosQueryHandler(ITodoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TodoDto>> Handle(GetAllTodosQuery request, CancellationToken ct)
    {
        var todos = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<TodoDto>>(todos);
    }
}