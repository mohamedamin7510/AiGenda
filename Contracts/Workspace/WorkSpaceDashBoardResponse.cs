namespace AI_genda_API.Contracts.Workspace;

public record WorkspaceDashboardResponse(
    DashboardStatsResponse Stats,
    WeeklyFocusTimeResponse WeeklyFocusTime
);