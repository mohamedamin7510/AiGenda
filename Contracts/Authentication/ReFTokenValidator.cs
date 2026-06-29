
namespace AI_genda_API.Contracts.Authentication;

public class ReFTokenValidator : AbstractValidator<ReFTokenReq>
{
    public ReFTokenValidator()
    {
        RuleFor(x=>x.Token).NotEmpty();
        RuleFor(x=>x.RefreshToken).NotEmpty();
        
    }
}
