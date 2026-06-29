namespace AI_genda_API.Contracts.SubTask;

public record SubTaskResponse(
    string Id,
    string Title,
    bool IsCompleted,
    DateTime CreatedAt
);
