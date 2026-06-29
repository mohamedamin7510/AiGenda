namespace AI_genda_API.Contracts.Workspace;

using FluentValidation;

public class UpdateWorkspaceMemberPermissionsRequestValidator : AbstractValidator<UpdateWorkspaceMemberPermissionsRequest>
{
    public UpdateWorkspaceMemberPermissionsRequestValidator()
    { 

        RuleForEach(x => x.Permissions)
            .NotEmpty();
    }
}