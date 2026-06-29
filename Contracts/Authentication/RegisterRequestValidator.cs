using AI_genda_API.Abstractions.Const;

namespace AI_genda_API.Contracts.Authentication;

public class RegisterRequestValidator:AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
          .Matches(RegexPattern.Password);

        RuleFor(x => x.ConfirmPassword)
         .Equal(x=>x.Password)
         .WithMessage("Password and confirmation password must match.");


    }

}
