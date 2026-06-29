namespace AI_genda_API.Contracts.Workspace;

public record WorkspaceDashboardCardsResponse(
    int TotalTasks,
    int NewTasksToday,
    double FocusTimeHours,
    double AverageFocusHoursPerDay,
    int ActiveSpaces,
    int ActiveCollaborators,
    int ProductivityScore
);