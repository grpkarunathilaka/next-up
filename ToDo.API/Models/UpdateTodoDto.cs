namespace TodoApi.Models;

public class UpdateTodoDto
{
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
    public string Priority { get; set; }
    public string? Category { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string>? Tags { get; set; }
}