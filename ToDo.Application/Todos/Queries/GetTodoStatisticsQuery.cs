using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Domain.Interfaces;

namespace ToDo.Application.Todos.Queries;
public record GetTodoStatisticsQuery() : IRequest<TodoStatisticsDto>;

public class GetTodoStatisticsQueryHandler : IRequestHandler<GetTodoStatisticsQuery, TodoStatisticsDto>
{
    private readonly ITodoRepository _repository;

    public GetTodoStatisticsQueryHandler(ITodoRepository repository) => _repository = repository;

    public async Task<TodoStatisticsDto> Handle(GetTodoStatisticsQuery request, CancellationToken ct)
    {
        var total = await _repository.GetTotalCountAsync();
        var completed = await _repository.GetCompletedCountAsync();
        var percentage = total == 0 ? 0 : Math.Round((double)completed / total * 100, 2);

        return new TodoStatisticsDto(total, completed, percentage);
    }
}