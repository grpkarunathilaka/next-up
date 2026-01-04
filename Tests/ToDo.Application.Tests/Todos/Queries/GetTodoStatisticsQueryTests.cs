using Moq;
using ToDo.Application.Todos.Queries;
using TodoApp.Domain.Interfaces;
using Xunit;

namespace ToDo.Application.Tests.Todos.Queries;

public class GetTodoStatisticsQueryTests
{
    [Fact]
    public async Task Handle_ShouldReturnCorrectStatistics_WhenTodosExist()
    {
        // Arrange
        var mockTodoRepository = new Mock<ITodoRepository>();
        mockTodoRepository.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(10);
        mockTodoRepository.Setup(r => r.GetCompletedCountAsync()).ReturnsAsync(4);

        var query = new GetTodoStatisticsQuery();
        var handler = new GetTodoStatisticsQueryHandler(mockTodoRepository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(4, result.CompletedCount);
        Assert.Equal(40.00, result.CompletionPercentage);
        mockTodoRepository.Verify(r => r.GetTotalCountAsync(), Times.Once);
        mockTodoRepository.Verify(r => r.GetCompletedCountAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnZeroPercentage_WhenTotalCountIsZero()
    {
        // Arrange
        var mockTodoRepository = new Mock<ITodoRepository>();
        mockTodoRepository.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(0);
        mockTodoRepository.Setup(r => r.GetCompletedCountAsync()).ReturnsAsync(0); // Can be any value, but 0 makes sense

        var query = new GetTodoStatisticsQuery();
        var handler = new GetTodoStatisticsQueryHandler(mockTodoRepository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.CompletedCount);
        Assert.Equal(0.00, result.CompletionPercentage);
        mockTodoRepository.Verify(r => r.GetTotalCountAsync(), Times.Once);
        mockTodoRepository.Verify(r => r.GetCompletedCountAsync(), Times.Once);
    }
}
