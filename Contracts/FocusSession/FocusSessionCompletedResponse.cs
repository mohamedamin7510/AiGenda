namespace AI_genda_API.Contracts.FocusSession;

public record FocusSessionCompletedResponse(
    string SessionId,
    int FocusDurationMinutes,
    int TaskProgressDeltaPercent,
    int Interruptions,
    int ProductivityScore,
    string ProductivityLabel,
    string AiInsight
);