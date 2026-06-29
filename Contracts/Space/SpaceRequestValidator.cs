namespace AI_genda_API.Contracts.Space;

public class SpaceRequestValidator : AbstractValidator<SpaceRequest>
{
    public SpaceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.IconCode)
            .NotEmpty()
            .MaximumLength(500);


    }
}