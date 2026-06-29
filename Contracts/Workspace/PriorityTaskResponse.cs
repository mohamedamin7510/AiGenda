using AI_genda_API.Abstractions.Enums;
using TaskStatus = AI_genda_API.Abstractions.Enums.TaskStatuss;

namespace AI_genda_API.Contracts.Workspace;

public record PriorityTaskResponse(
    string Id,
    string Title,
    TaskPriority Priority,
    TaskStatus Status,
    DateTime? DueDate
);