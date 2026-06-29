namespace AI_genda_API.Contracts.Space;

public class MoveSpaceRequestValidator : AbstractValidator<MoveSpaceRequest>
{
    public MoveSpaceRequestValidator()
    {
        RuleFor(x => x.TargetWorkspaceId)
            .GreaterThan(0);
    }
}