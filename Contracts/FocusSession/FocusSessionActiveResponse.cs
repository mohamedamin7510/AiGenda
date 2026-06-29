namespace AI_genda_API.Contracts.FocusSession;

public record FocusSessionActiveResponse(
    string SessionId,
    string TaskName,
    string TeamName,
    string? TaskDescription,
    int DurationMinutes,
    string AmbientSound,
    bool BreakAfter,
    bool BlockNotifications,
    DateTime StartedAt,
    DateTime? EndedAt,
    string Status,
    int Interruptions,
    int CompletedSubtasks,
    int TotalSubtasks,
    double ProgressPercent,
    string SubtasksLabel,
    List<FocusSessionSubTaskResponse> Subtasks
);