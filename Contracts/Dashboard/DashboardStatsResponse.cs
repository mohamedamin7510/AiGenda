namespace AI_genda_API.Contracts.Dashboard;

public record DashboardStatsResponse
(
    int TotalTasks,
    int CompletedTasks,
    int InProgressTasks,
    int TodoTasks,
    int OverdueTasks,
    int FocusSessionsThisWeek,
    double FocusTimeHours,
    int FocusCompletionRate,
    int ProductivityScore
);