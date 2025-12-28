using AutoMapper;
using MediatR;
using TodoApp.Domain.Interfaces;

namespace ToDo.Application.Todos.Queries;
public record SearchTodosQuery(string Query) : IRequest<IEnumerable<TodoDto>>;

public class SearchTodosQueryHandler : IRequestHandler<SearchTodosQuery, IEnumerable<TodoDto>>
{
    private readonly ITodoRepository _repository;
    private readonly IMapper _mapper;

    public SearchTodosQueryHandler(ITodoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TodoDto>> Handle(SearchTodosQuery request, CancellationToken ct)
    {
        var results = await _repository.SearchAsync(request.Query);
        return _mapper.Map<IEnumerable<TodoDto>>(results);
    }
}
