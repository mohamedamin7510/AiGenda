public record SpaceDetailResponse(
    string Id,
    string Name,
    string? Description,
    string IconCode,
    DateOnly LastActivity,
    bool IsPublic,
    int TotalTasks,
    int CompletedTasks
);
