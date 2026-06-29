namespace AI_genda_API.Contracts.Workspace;

public record WorkspaceMemberResponse
(
    string UserId,
    string FullName,
    string Email,
    bool IsOwner,
    DateTime JoinedAt,
    List<string> Permissions
);  