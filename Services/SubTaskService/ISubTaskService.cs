using AI_genda_API.Contracts.SubTask;

namespace AI_genda_API.Services.SubTaskService;

public interface ISubTaskService
{
    Task<Result<SubTaskResponse>> AddSubTaskAsync(int WorkspaceId, string SpaceId, string TaskId, string UserId, SubTaskRequest request, CancellationToken cancellationToken = default!);
    Task<Result<SubTaskResponse>> UpdateSubTaskAsync(int WorkspaceId, string SpaceId, string TaskId, string Id, string UserId, SubTaskRequest request, CancellationToken cancellationToken = default!);
    Task<Result> UpdateSubTaskStatusAsync(int WorkspaceId, string SpaceId, string TaskId, string Id, string UserId, UpdateSubTaskStatusRequest request, CancellationToken cancellationToken = default!);
    Task<Result> DeleteSubTaskAsync(int WorkspaceId, string SpaceId, string TaskId, string Id, string UserId, CancellationToken cancellationToken = default!);
}


