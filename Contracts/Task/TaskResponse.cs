using AI_genda_API.Abstractions.Enums;
using AI_genda_API.Contracts.SubTask;
using TaskStatus = AI_genda_API.Abstractions.Enums.TaskStatuss;
namespace AI_genda_API.Contracts.Task;

public record TaskResponse(
    string Id,
    string Title,
    string? Description,
    TaskStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    DateTime CreatedAt,
    List<AssigneeResponse> Assignees,
    List<SubTaskResponse> SubTasks
);
