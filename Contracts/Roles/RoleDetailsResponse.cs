namespace AI_genda_API.Contracts.Roles;

public record RoleDetailsResponse
(
    string Id,
    string Name,
    bool IsDeleted,
    IEnumerable<string> Permissions
);
