using AI_genda_API.Abstractions.Enums;
using TaskStatus = AI_genda_API.Abstractions.Enums.TaskStatuss;

namespace AI_genda_API.Contracts.Task;

public record RemovedTaskResponse(
    string Id,
    string Title,
    string? Description,
    TaskStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime RemovedAt
);
