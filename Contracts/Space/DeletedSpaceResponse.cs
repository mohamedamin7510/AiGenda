namespace AI_genda_API.Contracts.Space;

public record DeletedSpaceResponse
(
    string Id,
    string Name,
    string? Description,
    string IconCode,
    DateOnly LastActivity,
    DateTime RemovedAt,
    int TotalTasks
);