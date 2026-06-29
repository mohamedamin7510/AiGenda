using AI_genda_API.Abstractions.Const;

namespace BucketSurvey.Api.Contract.Authentication;

public class ResetPasswordRequestvalidator:AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestvalidator()
    {
        RuleFor(x => x.email)
            .NotEmpty().EmailAddress();

        RuleFor(x => x.code)
            .NotEmpty();

        RuleFor(x => x.newpassword)
            .NotEmpty()
            .Matches(RegexPattern.Password);

    }
}
