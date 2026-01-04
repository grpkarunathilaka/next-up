using AutoMapper;
using Moq;
using ToDo.Application.Todos.Queries;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;
using Xunit;

namespace ToDo.Application.Tests.Todos.Queries;

public class GetAllTodosQueryTests
{
    [Fact]
    public async Task Handle_ShouldReturnAllTodoItemsMappedToDto()
    {
        // Arrange
        var mockTodoRepository = new Mock<ITodoRepository>();
        var mockMapper = new Mock<IMapper>();

        var todoItem1 = new TodoItem("Task 1");
        var todoItem2 = new TodoItem("Task 2");

        var todoItems = new List<TodoItem>
        {
            todoItem1,
            todoItem2
        };

        var todoDtos = new List<TodoDto>
        {
            new TodoDto(todoItem1.Id, "Task 1", false, 0, "medium"),
            new TodoDto(todoItem2.Id, "Task 2", false, 0, "medium")
        };

        mockTodoRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(todoItems);
        mockMapper.Setup(m => m.Map<IEnumerable<TodoDto>>(It.IsAny<IEnumerable<TodoItem>>()))
                  .Returns(todoDtos);

        var query = new GetAllTodosQuery();
        var handler = new GetAllTodosQueryHandler(mockTodoRepository.Object, mockMapper.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(todoDtos.Count, result.Count());
        Assert.Equal(todoDtos, result);
        mockTodoRepository.Verify(r => r.GetAllAsync(), Times.Once);
        mockMapper.Verify(m => m.Map<IEnumerable<TodoDto>>(It.IsAny<IEnumerable<TodoItem>>()), Times.Once);
    }
}
