namespace ToDo.Application.Todos.Queries;

public record TodoDto(string Id, string Title, bool IsCompleted, int Position, string Priority);
public record TodoStatisticsDto(int TotalCount, int CompletedCount, double CompletionPercentage);