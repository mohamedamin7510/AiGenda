namespace AI_genda_API.Contracts.Workspace;

public record WorkspaceByIdDashboardResponse(
    string Userdisplayname,
    int ProductivityDeltapercent,
    WorkspaceDashboardCardsResponse Cards,
    WeeklyFocusTimeResponse Weeklyfocustime,
    List<WorkspaceRecentActivityResponse> Recentactivities,
    List<WorkspaceDashboardPriorityTaskResponse> Prioritytasks
);