namespace AI_genda_API.Contracts.FocusSession;

public class FocusSessionStartRequestValidator : AbstractValidator<FocusSessionStartRequest>
{
    public FocusSessionStartRequestValidator()
    {
        RuleFor(x => x.DurationMinutes)
            .Must(x => x is 25 or 45 or 60 or 90)
            .WithMessage("Duration must be 25, 45, 60, or 90 minutes.");

        RuleFor(x => x.AmbientSound)
            .NotEmpty()
            .MaximumLength(80);
    }
}