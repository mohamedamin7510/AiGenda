namespace AI_genda_API.Contracts.FocusSession;

public record FocusSessionStartRequest(
    int DurationMinutes,
    string AmbientSound,
    bool BreakAfter,
    bool BlockNotifications
);