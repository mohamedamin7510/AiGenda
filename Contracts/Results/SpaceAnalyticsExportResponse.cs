namespace AI_genda_API.Contracts.Results;

public record SpaceAnalyticsExportResponse(
    byte[] Content,
    string ContentType,
    string FileName
);