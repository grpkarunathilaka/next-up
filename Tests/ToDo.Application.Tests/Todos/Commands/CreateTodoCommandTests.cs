using AutoMapper;
using Moq;
using ToDo.Application.Todos.Commands;
using ToDo.Application.Todos.Queries;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;
using Xunit;

namespace ToDo.Application.Tests.Todos.Commands;

public class CreateTodoCommandTests
{
    [Fact]
    public async Task Handle_ShouldAddTodoItemAndReturnTodoDto()
    {
        // Arrange
        var mockTodoRepository = new Mock<ITodoRepository>();
        var mockMapper = new Mock<IMapper>();

        var command = new CreateTodoCommand("Test Title", "High");
        var todoItem = new TodoItem("Test Title", priority: "High");
        var todoDto = new TodoDto("1", "Test Title", false, 0, "High");

        mockMapper.Setup(m => m.Map<TodoDto>(It.IsAny<TodoItem>()))
                  .Returns(todoDto);

        var handler = new CreateTodoCommandHandler(mockTodoRepository.Object, mockMapper.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockTodoRepository.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Once);
        mockMapper.Verify(m => m.Map<TodoDto>(It.IsAny<TodoItem>()), Times.Once);
        Assert.Equal(todoDto, result);
    }
}
