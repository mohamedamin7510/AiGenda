namespace AI_genda_API.Contracts.Roles;

public class RoleRequestValidator :AbstractValidator<RoleRequest>
{

    public RoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 250);


        RuleFor(x => x.Permissions)
            .NotEmpty()
            .Must(x => x.Distinct().Count() == x.Count())
            .WithMessage("{PropertyName}: This prop must be contains unique permissions!")
            .When(x=> x.Permissions is not null);

    }
}
