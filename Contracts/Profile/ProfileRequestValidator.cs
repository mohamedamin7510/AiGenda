namespace BucketSurvey.Api.Contract.User;

public class ProfileRequestValidator:AbstractValidator<ProfileRequest>
{
    public ProfileRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.SecondName)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .Must(date => date < DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("the Date Of Birth should be less than current date!");

        RuleFor(x => x.JobTitle) 
            .NotEmpty()
            .MaximumLength(50);

    }

}
