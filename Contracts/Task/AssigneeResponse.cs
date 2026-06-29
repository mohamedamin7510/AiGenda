namespace AI_genda_API.Contracts.Task;

public record AssigneeResponse(
    string UserId,
    string FullName,
    string? AvatarUrl
);
