using AI_genda_API.Abstractions.Enums;

namespace AI_genda_API.Contracts.Task;

public record TaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate
);







