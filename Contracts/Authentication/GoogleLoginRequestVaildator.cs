namespace BucketSurvey.Api.Contract.Authentication;
public class GoogleLoginRequestVaildator : AbstractValidator<GoogleLoginRequest>
{
    public GoogleLoginRequestVaildator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty()
            .WithMessage("IdToken is required.");
    }
}