using AutoMapper;
using Moq;
using ToDo.Application.Todos.Queries;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;
using Xunit;

namespace ToDo.Application.Tests.Todos.Queries;

public class SearchTodosQueryTests
{
    [Fact]
    public async Task Handle_ShouldReturnFilteredTodoItemsMappedToDto()
    {
        // Arrange
        var mockTodoRepository = new Mock<ITodoRepository>();
        var mockMapper = new Mock<IMapper>();

        var queryText = "Task 1";
        var todoItem1 = new TodoItem("Task 1 Title");
        var todoItem2 = new TodoItem("Another Task");

        var todoItems = new List<TodoItem> { todoItem1 }; // Only todoItem1 matches queryText

        var todoDtos = new List<TodoDto> { new TodoDto(todoItem1.Id, "Task 1 Title", false, 0, "medium") };

        mockTodoRepository.Setup(r => r.SearchAsync(queryText)).ReturnsAsync(todoItems);
        mockMapper.Setup(m => m.Map<IEnumerable<TodoDto>>(It.IsAny<IEnumerable<TodoItem>>()))
                  .Returns(todoDtos);

        var query = new SearchTodosQuery(queryText);
        var handler = new SearchTodosQueryHandler(mockTodoRepository.Object, mockMapper.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(todoDtos, result);
        mockTodoRepository.Verify(r => r.SearchAsync(queryText), Times.Once);
        mockMapper.Verify(m => m.Map<IEnumerable<TodoDto>>(todoItems), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoMatchingTodosFound()
    {
        // Arrange
        var mockTodoRepository = new Mock<ITodoRepository>();
        var mockMapper = new Mock<IMapper>();

        var queryText = "NonExistent";
        var emptyTodoItems = new List<TodoItem>();
        var emptyTodoDtos = new List<TodoDto>();

        mockTodoRepository.Setup(r => r.SearchAsync(queryText)).ReturnsAsync(emptyTodoItems);
        mockMapper.Setup(m => m.Map<IEnumerable<TodoDto>>(It.IsAny<IEnumerable<TodoItem>>()))
                  .Returns(emptyTodoDtos);

        var query = new SearchTodosQuery(queryText);
        var handler = new SearchTodosQueryHandler(mockTodoRepository.Object, mockMapper.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        mockTodoRepository.Verify(r => r.SearchAsync(queryText), Times.Once);
        mockMapper.Verify(m => m.Map<IEnumerable<TodoDto>>(emptyTodoItems), Times.Once);
    }
}
