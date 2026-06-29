namespace AI_genda_API.Contracts.FocusSession;

public record FocusSessionSubTaskResponse(
    string Id,
    string Title,
    bool IsCompleted
);