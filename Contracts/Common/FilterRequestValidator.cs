namespace AI_genda_API.Contracts.Common;

public class  FilterRequestValidator : AbstractValidator<FilterRequest> 
{

    public FilterRequestValidator()
    {

        RuleFor(x => x.PageNumber)
              .GreaterThan(0)
              .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .Must(x => x <= 20)
            .WithMessage("Page size number must be smaller than or equal to 20.");

        RuleFor(x => x.SortOrder)
            .Must(x => x == "asc" || x == "desc")
            .WithMessage("Sortorder accepts these two values asc and desc only.");

    }
}

