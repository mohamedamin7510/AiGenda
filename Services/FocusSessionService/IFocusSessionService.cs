using AI_genda_API.Contracts.FocusSession;

namespace AI_genda_API.Services.FocusSessionService;

public interface IFocusSessionService
{
    Task<Result<FocusSessionActiveResponse>> StartAsync(int WorkspaceId, string SpaceId, string TaskId, string UserId, FocusSessionStartRequest request, CancellationToken cancellationToken = default!);
    Task<Result<FocusSessionActiveResponse>> GetCurrentAsync(int WorkspaceId, string SpaceId, string TaskId, string UserId, CancellationToken cancellationToken = default!);
    Task<Result<FocusSessionPauseResponse>> PauseAsync(int WorkspaceId, string SpaceId, string TaskId, string SessionId, string UserId, CancellationToken cancellationToken = default!);
    Task<Result<FocusSessionActiveResponse>> ResumeAsync(int WorkspaceId, string SpaceId, string TaskId, string SessionId, string UserId, CancellationToken cancellationToken = default!);
    Task<Result<FocusSessionCompletedResponse>> CompleteAsync(int WorkspaceId, string SpaceId, string TaskId, string SessionId, string UserId, CancellationToken cancellationToken = default!);
    Task<Result> AbandonAsync(int WorkspaceId, string SpaceId, string TaskId, string SessionId, string UserId, CancellationToken cancellationToken = default!);
    Task<Result<FocusSessionActiveResponse>> ToggleSubTaskAsync(int WorkspaceId, string SpaceId, string TaskId, string SessionId, string SubTaskId, string UserId, ToggleFocusSessionSubTaskRequest request, CancellationToken cancellationToken = default!);
}