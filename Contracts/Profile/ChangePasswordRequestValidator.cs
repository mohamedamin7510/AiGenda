using AI_genda_API.Abstractions.Const;

namespace BucketSurvey.Api.Contract.User;

public class ChangePasswordRequestValidator: AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(RegexPattern.Password)
            .WithMessage("The password dosen't match the cratieria ")
            .NotEqual(x=>x.CurrentPassword)
            .WithMessage("You shouldn't repeat the old password!");
    }
}
