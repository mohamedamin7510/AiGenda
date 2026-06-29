namespace AI_genda_API.Contracts.Results;

public record SpaceAnalyticsResponse
(
    string SpaceId,
    string SpaceName,
    bool IsActive,
    int DaysRange,
    int TotalTasks,
    int CompletedTasks,
    List<TaskCompletionTrendPoint> TaskCompletionTrend,
    List<PriorityDistributionItem> PriorityDistribution,
    List<TeamProductivityItem> TeamProductivity
);
