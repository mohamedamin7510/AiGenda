namespace AI_genda_API.Contracts.Roles;

public record RoleRequest
(
    string Name,
        IEnumerable<string> Permissions 
);