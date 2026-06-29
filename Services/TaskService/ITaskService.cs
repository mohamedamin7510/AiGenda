namespace AI_genda_API.Services.TaskService;

public interface ITaskService
{

    Task<Result<TaskResponse>> AddAsync(int WorkspaceId, string SpaceId, string UserId, TaskRequest request, CancellationToken cancellationToken = default!);
    Task<Result<PaginatedList<TaskResponse>>> GetAllAsync(int WorkspaceId, string SpaceId, string UserId, FilterRequest filterRequest, CancellationToken cancellationToken = default!);
    Task<Result<IEnumerable<RemovedTaskResponse>>> GetAllRemovedAsync(int WorkspaceId, string SpaceId, string UserId, CancellationToken cancellationToken = default!);
    Task<Result<RemovedTaskResponse>> GetRemovedByIdAsync(int WorkspaceId, string SpaceId, string Id, string UserId, CancellationToken cancellationToken = default!);
    Task<Result<TaskResponse>> GetByIdAsync(int WorkspaceId, string SpaceId, string Id, string UserId, CancellationToken cancellationToken = default!);
    Task<Result<TaskResponse>> UpdateAsync(int WorkspaceId, string SpaceId, string Id, string UserId, TaskRequest request, CancellationToken cancellationToken = default!);
    Task<Result> UpdateStatusAsync(int WorkspaceId, string SpaceId, string Id, string UserId, UpdateTaskStatusRequest request, CancellationToken cancellationToken = default!);
    Task<Result> DeleteAsync(int WorkspaceId, string SpaceId, string Id, string UserId, CancellationToken cancellationToken = default!);
    Task<Result> RestoreAsync(int WorkspaceId, string SpaceId, string Id, string UserId, CancellationToken cancellationToken = default!);
    Task<Result> AssignMemberAsync(int WorkspaceId, string SpaceId, string Id, string UserId, AssignTaskRequest request, CancellationToken cancellationToken = default!);
    Task<Result> UnAssignMemberAsync(int WorkspaceId, string SpaceId, string Id, string UserId, AssignTaskRequest request, CancellationToken cancellationToken = default!);

}
