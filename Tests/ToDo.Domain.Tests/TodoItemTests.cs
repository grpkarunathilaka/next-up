using TodoApp.Domain.Entities;
using TodoApp.Domain.Events;
using Xunit;
using System.Linq;

namespace ToDo.Domain.Tests;

public class TodoItemTests
{
    [Fact]
    public void Should_Create_TodoItem_With_Valid_Title()
    {
        // Arrange
        var title = "Test Todo";

        // Act
        var todoItem = new TodoItem(title);

        // Assert
        Assert.Equal(title, todoItem.Title);
        Assert.False(todoItem.IsCompleted);
        Assert.Single(todoItem.DomainEvents);
        Assert.IsType<TodoCreatedEvent>(todoItem.DomainEvents.First());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Throw_ArgumentException_For_Invalid_Title(string? invalidTitle)
    {
        // Act & Assert
#pragma warning disable CS8604 // Possible null reference argument.
        Assert.Throws<ArgumentException>(() => new TodoItem(invalidTitle));
#pragma warning restore CS8604 // Possible null reference argument.
    }

    [Fact]
    public void Should_Update_Details()
    {
        // Arrange
        var todoItem = new TodoItem("Initial Title");

        // Act
        todoItem.UpdateDetails("Updated Title", true, "high");

        // Assert
        Assert.Equal("Updated Title", todoItem.Title);
        Assert.True(todoItem.IsCompleted);
    }

    [Fact]
    public void Should_Set_Position()
    {
        // Arrange
        var todoItem = new TodoItem("Test Todo");

        // Act
        todoItem.SetPosition(10);

        // Assert
        Assert.Equal(10, todoItem.Position);
    }
}
