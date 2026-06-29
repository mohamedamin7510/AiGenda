namespace BucketSurvey.Api.Contract.Authentication;

public class ConfirmEmailRequestValidator :AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.userId)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}
