namespace AI_genda_API.Contracts.FocusSession;

public record FocusSessionPauseResponse(
    string SessionId,
    double SessionProgressPercent,
    string TimeRemaining,
    string DailyGoalLabel
);