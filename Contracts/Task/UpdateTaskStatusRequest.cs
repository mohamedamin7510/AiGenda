using TaskStatus = AI_genda_API.Abstractions.Enums.TaskStatuss;

namespace AI_genda_API.Contracts.Task;
public record UpdateTaskStatusRequest(
    TaskStatus Status
);
