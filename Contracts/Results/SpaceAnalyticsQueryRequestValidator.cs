namespace AI_genda_API.Contracts.Results;

public class SpaceAnalyticsQueryRequestValidator : AbstractValidator<SpaceAnalyticsQueryRequest>
{
    public SpaceAnalyticsQueryRequestValidator()
    {
        RuleFor(x => x.Days)
            .Must(x => x is 7 or 30 or 90)
            .WithMessage("Days must be 7, 30, or 90.");

        RuleFor(x => x.Search)
            .MaximumLength(100);
    }
}