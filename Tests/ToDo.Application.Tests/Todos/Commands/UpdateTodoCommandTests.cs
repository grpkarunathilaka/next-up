using AutoMapper;
using Moq;
using ToDo.Application.Todos.Commands;
using ToDo.Application.Todos.Queries;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;
using Xunit;

namespace ToDo.Application.Tests.Todos.Commands;

public class UpdateTodoCommandTests
{
    [Fact]
    public async Task Handle_ShouldUpdateTodoItemAndReturnTodoDto_WhenTodoItemExists()
    {
        // Arrange
        var mockTodoRepository = new Mock<ITodoRepository>();
        var mockMapper = new Mock<IMapper>();

        var todoItem = new TodoItem("Original Title", priority: "High"); // Id will be generated here
        var command = new UpdateTodoCommand { Id = todoItem.Id, Title = "Updated Title", IsCompleted = true, Priority = "Low" };
        var updatedTodoDto = new TodoDto(todoItem.Id, "Updated Title", true, 0, "Low");

        mockTodoRepository.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(todoItem);
        // We need to verify that UpdateDetails is called on the entity, not on a mock
        // So we'll let the actual UpdateDetails method be called on our todoItem instance.
        mockTodoRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>())).Returns(Task.CompletedTask);
        mockMapper.Setup(m => m.Map<TodoDto>(It.IsAny<TodoItem>()))
                  .Returns(updatedTodoDto);

        var handler = new UpdateTodoCommandHandler(mockTodoRepository.Object, mockMapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedTodoDto, result);
        mockTodoRepository.Verify(r => r.GetByIdAsync(todoItem.Id), Times.Once);
        mockTodoRepository.Verify(r => r.UpdateAsync(It.Is<TodoItem>(t => t.Id == todoItem.Id && t.Title == "Updated Title" && t.IsCompleted == true && t.Priority == "Low")), Times.Once);
        mockMapper.Verify(m => m.Map<TodoDto>(It.Is<TodoItem>(t => t.Id == todoItem.Id && t.Title == "Updated Title" && t.IsCompleted == true && t.Priority == "Low")), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenTodoItemDoesNotExist()
    {
        // Arrange
        var mockTodoRepository = new Mock<ITodoRepository>();
        var mockMapper = new Mock<IMapper>();

        var command = new UpdateTodoCommand { Id = "2", Title = "Updated Title" };

        mockTodoRepository.Setup(r => r.GetByIdAsync("2")).ReturnsAsync(null as TodoItem);

        var handler = new UpdateTodoCommandHandler(mockTodoRepository.Object, mockMapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
        mockTodoRepository.Verify(r => r.GetByIdAsync("2"), Times.Once);
        mockTodoRepository.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Never);
        mockMapper.Verify(m => m.Map<TodoDto>(It.IsAny<TodoItem>()), Times.Never);
    }
}
