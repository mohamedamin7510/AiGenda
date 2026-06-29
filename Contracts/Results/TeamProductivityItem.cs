namespace AI_genda_API.Contracts.Results;

public record TeamProductivityItem
(
    string UserId, 
    string MemberName, 
    string? AvatarUrl, 
    int CompletedTasks
);
