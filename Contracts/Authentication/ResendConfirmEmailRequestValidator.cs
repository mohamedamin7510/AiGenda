namespace BucketSurvey.Api.Contract.Authentication;

public class ResendConfirmEmailRequestValidator:AbstractValidator<ResendConfirmEmailRequest>
{
    public ResendConfirmEmailRequestValidator()
    {

        RuleFor(x => x.email)
            .NotEmpty()
            .EmailAddress();

    }
}
