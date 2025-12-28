namespace TodoApi.Models;

using FluentValidation;

public class UpdateTodoDto
{
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
    public string? Priority { get; set; }
    public string? Category { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string>? Tags { get; set; }
}

public class UpdateTodoDtoValidator : AbstractValidator<UpdateTodoDto>
{
    public UpdateTodoDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().When(x => x.Title != null).WithMessage("Title cannot be empty if provided.");

        RuleFor(x => x.Priority)
            .NotEmpty().When(x => x.Priority != null).WithMessage("Priority cannot be empty if provided.");
            // TODO: Add further validation for specific priority values (e.g., Enum.IsDefined)

        RuleFor(x => x.Category)
            .NotEmpty().When(x => x.Category != null).WithMessage("Category cannot be empty if provided.");
        
        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.All(tag => !string.IsNullOrWhiteSpace(tag)))
            .WithMessage("Tags cannot contain empty or whitespace-only entries.");
    }
}