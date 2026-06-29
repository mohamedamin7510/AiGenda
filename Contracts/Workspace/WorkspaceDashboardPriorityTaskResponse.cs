using AI_genda_API.Abstractions.Enums;

namespace AI_genda_API.Contracts.Workspace;

public record WorkspaceDashboardPriorityTaskResponse(
    string Id,
    string Title,
    TaskPriority Priority,
    TaskStatuss Status,
    DateTime? DueDate,
    string SpaceName
);