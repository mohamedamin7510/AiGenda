using AI_genda_API.Abstractions.Enums;

namespace AI_genda_API.Contracts.Workspace;

public record DeletedWorkSpaceResponse
(
    int Id,
    string Name,
    string Description,
    string IconCode,
    WorkSpaceVisibility Visibility,
    DateTime RemovedAt,
    int NumberofMembers,
    int NumberofTasks
);