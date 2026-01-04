using Moq;
using ToDo.Application.Todos.Commands;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;
using Xunit;

namespace ToDo.Application.Tests.Todos.Commands;

public class DeleteTodoCommandTests
{
    [Fact]
    public async Task Handle_ShouldDeleteTodoItem_WhenTodoItemExists()
    {
        // Arrange
        var mockTodoRepository = new Mock<ITodoRepository>();
        var command = new DeleteTodoCommand("1");
        var todoItem = new TodoItem("Test Title");

        mockTodoRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(todoItem);
        mockTodoRepository.Setup(r => r.DeleteAsync(todoItem)).Returns(Task.CompletedTask);

        var handler = new DeleteTodoCommandHandler(mockTodoRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        mockTodoRepository.Verify(r => r.GetByIdAsync("1"), Times.Once);
        mockTodoRepository.Verify(r => r.DeleteAsync(todoItem), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenTodoItemDoesNotExist()
    {
        // Arrange
        var mockTodoRepository = new Mock<ITodoRepository>();
        var command = new DeleteTodoCommand("2");

        mockTodoRepository.Setup(r => r.GetByIdAsync("2")).ReturnsAsync(null as TodoItem);

        var handler = new DeleteTodoCommandHandler(mockTodoRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        mockTodoRepository.Verify(r => r.GetByIdAsync("2"), Times.Once);
        mockTodoRepository.Verify(r => r.DeleteAsync(It.IsAny<TodoItem>()), Times.Never);
    }
}
