namespace AI_genda_API.Contracts.Workspace;

public record UpdateWorkspaceMemberPermissionsRequest
(
    List<string> Permissions
);