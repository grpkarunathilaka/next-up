using AutoMapper;
using Moq;
using ToDo.Application.Todos.Queries;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;
using Xunit;

namespace ToDo.Application.Tests.Todos.Queries;

public class GetTodoByIdQueryTests
{
    [Fact]
    public async Task Handle_ShouldReturnTodoDto_WhenTodoItemExists()
    {
        // Arrange
        var mockTodoRepository = new Mock<ITodoRepository>();
        var mockMapper = new Mock<IMapper>();

        var todoItem = new TodoItem("Test Task");
        var todoDto = new TodoDto(todoItem.Id, "Test Task", false, 0, "medium");

        var query = new GetTodoByIdQuery(todoItem.Id);

        mockTodoRepository.Setup(r => r.GetByIdAsync(todoItem.Id)).ReturnsAsync(todoItem);
        mockMapper.Setup(m => m.Map<TodoDto>(It.IsAny<TodoItem>()))
                  .Returns(todoDto);

        var handler = new GetTodoByIdQueryHandler(mockTodoRepository.Object, mockMapper.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(todoDto, result);
        mockTodoRepository.Verify(r => r.GetByIdAsync(todoItem.Id), Times.Once);
        mockMapper.Verify(m => m.Map<TodoDto>(todoItem), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenTodoItemDoesNotExist()
    {
        // Arrange
        var mockTodoRepository = new Mock<ITodoRepository>();
        var mockMapper = new Mock<IMapper>();

        var nonExistentId = "nonExistentId";
        var query = new GetTodoByIdQuery(nonExistentId);

        mockTodoRepository.Setup(r => r.GetByIdAsync(nonExistentId)).ReturnsAsync(null as TodoItem);

        var handler = new GetTodoByIdQueryHandler(mockTodoRepository.Object, mockMapper.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        mockTodoRepository.Verify(r => r.GetByIdAsync(nonExistentId), Times.Once);
        mockMapper.Verify(m => m.Map<TodoDto>(It.IsAny<TodoItem>()), Times.Never);
    }
}
