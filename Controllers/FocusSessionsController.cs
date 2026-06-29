using AI_genda_API.Contracts.FocusSession;
using AI_genda_API.Services.FocusSessionService;

namespace AI_genda_API.Controllers;

[ApiController]
[Route("api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/Tasks/{TaskId}/[controller]")]
[Authorize(Roles = DefaultRoles.Member)]
public class FocusSessionsController(IFocusSessionService focusSessionService) : ControllerBase
{
    private readonly IFocusSessionService _FocusSessionService = focusSessionService;

    [HttpPost]
    [HasPermission(Permissions.UpdateTasks)]
    public async Task<IActionResult> Start([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string TaskId, [FromBody] FocusSessionStartRequest request, CancellationToken cancellationToken = default!)
    {
        var result = await _FocusSessionService.StartAsync(WorkspaceId, SpaceId, TaskId, User.GetUserId()!, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("current")]
    [HasPermission(Permissions.GetTasks)]
    public async Task<IActionResult> GetCurrent([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string TaskId, CancellationToken cancellationToken = default!)
    {
        var result = await _FocusSessionService.GetCurrentAsync(WorkspaceId, SpaceId, TaskId, User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{SessionId}/pause")]
    [HasPermission(Permissions.UpdateTasks)]
    public async Task<IActionResult> Pause([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string TaskId, [FromRoute] string SessionId, CancellationToken cancellationToken = default!)
    {
        var result = await _FocusSessionService.PauseAsync(WorkspaceId, SpaceId, TaskId, SessionId, User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{SessionId}/resume")]
    [HasPermission(Permissions.UpdateTasks)]
    public async Task<IActionResult> Resume([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string TaskId, [FromRoute] string SessionId, CancellationToken cancellationToken = default!)
    {
        var result = await _FocusSessionService.ResumeAsync(WorkspaceId, SpaceId, TaskId, SessionId, User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{SessionId}/complete")]
    [HasPermission(Permissions.UpdateTasks)]
    public async Task<IActionResult> Complete([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string TaskId, [FromRoute] string SessionId, CancellationToken cancellationToken = default!)
    {
        var result = await _FocusSessionService.CompleteAsync(WorkspaceId, SpaceId, TaskId, SessionId, User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{SessionId}/abandon")]
    [HasPermission(Permissions.UpdateTasks)]
    public async Task<IActionResult> Abandon([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string TaskId, [FromRoute] string SessionId, CancellationToken cancellationToken = default!)
    {
        var result = await _FocusSessionService.AbandonAsync(WorkspaceId, SpaceId, TaskId, SessionId, User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{SessionId}/subtasks/{SubTaskId}")]
    [HasPermission(Permissions.UpdateTasks)]
    public async Task<IActionResult> ToggleSubTask([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string TaskId, [FromRoute] string SessionId, [FromRoute] string SubTaskId, [FromBody] ToggleFocusSessionSubTaskRequest request, CancellationToken cancellationToken = default!)
    {
        var result = await _FocusSessionService.ToggleSubTaskAsync(WorkspaceId, SpaceId, TaskId, SessionId, SubTaskId, User.GetUserId()!, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
