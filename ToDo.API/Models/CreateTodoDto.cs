using FluentValidation;

namespace TodoApi.Models;

public class CreateTodoDto
{
    public required string Title { get; set; }
    public string Priority { get; set; } = "medium";
    public string? Category { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string>? Tags { get; set; }
}

public class CreateTodoDtoValidator : AbstractValidator<CreateTodoDto>
{
    public CreateTodoDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
            .MinimumLength(1).WithMessage("Title must be at least 1 character");

        RuleFor(x => x.Category)
            .MaximumLength(50).When(x => x.Category != null);

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.Today).When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be today or in the future");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("Maximum 10 tags allowed");
    }
}
