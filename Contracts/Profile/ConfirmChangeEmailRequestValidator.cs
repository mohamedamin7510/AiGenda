namespace AI_genda_API.Contracts.Profile;

public class ConfirmChangeEmailRequestValidator: AbstractValidator<ConfirmChangeEmailRequest>
{
    public ConfirmChangeEmailRequestValidator()
    {
        RuleFor(x => x.newemail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty();
        
    }
}
