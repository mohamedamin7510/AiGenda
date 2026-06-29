namespace AI_genda_API.Contracts.Results;

public record SpaceAnalyticsQueryRequest
{
    public int Days { get; set; } = 30;
    public string? Search { get; set; }
}