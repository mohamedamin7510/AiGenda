namespace AI_genda_API.Contracts.SubTask;

public class SubTaskRequestValidator : AbstractValidator<SubTaskRequest>
{
    public SubTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(250);
    }
}