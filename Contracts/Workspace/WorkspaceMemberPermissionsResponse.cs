namespace AI_genda_API.Contracts.Workspace;

public record WorkspaceMemberPermissionsResponse
(
    string UserId,
    bool IsOwner,
    List<string> Permissions
);