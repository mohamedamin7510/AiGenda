using AI_genda_API.Abstractions.Enums;

namespace AI_genda_API.Contracts.Task;

public class TaskRequestValidator : AbstractValidator<TaskRequest>
{
    public TaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(x => x.Description)
            .Length(1, 400)
            .NotEmpty()
            .WithMessage("Description is required.");


        RuleFor(x => x.Priority)
             .NotEmpty()
            .Must(x=>x == TaskPriority.Low 
                || x == TaskPriority.Medium || x == TaskPriority.High || x == TaskPriority.Critical)
            .WithMessage("{PropertyName}: should be 0 for Low, 1 for Medium, 2 for High, or 3 for Critical.");



        RuleFor(x => x.DueDate)
            .Must(x => x > DateTime.UtcNow)
            .WithMessage("DueDate must be in the future.");
    }

}
