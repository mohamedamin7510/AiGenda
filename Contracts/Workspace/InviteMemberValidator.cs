namespace AI_genda_API.Contracts.Workspace;

public class InviteMemberValidator : AbstractValidator<InviteMember>
{
    public InviteMemberValidator()
    {
        RuleFor(x => x.email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");


    }



}
