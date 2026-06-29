namespace AI_genda_API.Contracts.Authentication;

public class LoginReqValidator:AbstractValidator<LoginReq>
{
    public LoginReqValidator()
    {
        RuleFor(x => x.Email).NotEmpty()
            .EmailAddress().WithMessage("{PropertyName} should be in an email format." );

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Invalid Password , the {PropertyName} Property shouldn't be empty" );

    }
}
