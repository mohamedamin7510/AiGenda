namespace AI_genda_API.Contracts.Workspace;

public record WorkspaceRecentActivityResponse(
    string Message,
    DateTime OccurredAt,
    string Accent
);