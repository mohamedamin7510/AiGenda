
namespace AI_genda_API.Contracts.Workspace;

using FluentValidation;

public class WorkSpaceRequestValidator : AbstractValidator<WorkSpaceRequest>
{
    public WorkSpaceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(x => x.Description)
            .MaximumLength(300);

        RuleFor(x => x.IconCode)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Visibility)
            .NotNull()
            .Must(x => (int)x == 0 || (int)x == 1 || (int)x == 2)
            .WithMessage("0:Private - 1:Team - 2:Public --- Match these numbers only");          
    }



}
