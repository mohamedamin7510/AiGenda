namespace AI_genda_API.Contracts.Profile;

public class ChangeEmailRequestValidator:AbstractValidator<changeEmailRequest>
{
    public ChangeEmailRequestValidator()
    {
        RuleFor(x=> x.newemail)
            .NotEmpty()
            .EmailAddress();
    }

}
