using AI_genda_API.Abstractions.Const;
using AI_genda_API.Contracts.SubTask;
using AI_genda_API.Services.SubTaskService;

namespace AI_genda_API.Controllers;

[Route("api/WorkSpaces/{WorkspaceId}/Spaces/{SpaceId}/Tasks/{TaskId}/[controller]")]
[ApiController]
public class SubTasksController(ISubTaskService subTaskService) : ControllerBase
{
    private readonly ISubTaskService _SubTaskService = subTaskService;

    [HttpPost()]
    [HasPermission(Permissions.UpdateTasks)]
    public async Task<IActionResult> AddSubTask([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string TaskId, [FromBody] SubTaskRequest request, CancellationToken cancellationToken = default!)
    {

        var result = await _SubTaskService.AddSubTaskAsync(WorkspaceId, SpaceId, TaskId, User.GetUserId()!, request, cancellationToken);

        var URI = $"/api/workspaces/{WorkspaceId}/spaces/{SpaceId}/Tasks/{TaskId}";

        return result.IsSuccess ? Created(URI , result.Value ) : result.ToProblem();
    }




    [HttpPut("{Id}")]
    [HasPermission(Permissions.UpdateTasks)]
    public async Task<IActionResult> UpdateSubTask([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string TaskId, [FromRoute] string Id, [FromBody] SubTaskRequest request, CancellationToken cancellationToken = default!)
    {
        var result = await _SubTaskService.UpdateSubTaskAsync(WorkspaceId, SpaceId, TaskId, Id, User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }



    [HttpPut("{Id}/status")]
    [HasPermission(Permissions.UpdateTasks)]
    public async Task<IActionResult> UpdateSubTaskStatus([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string TaskId, [FromRoute] string Id, [FromBody] UpdateSubTaskStatusRequest request, CancellationToken cancellationToken = default!)
    {
        var result = await _SubTaskService.UpdateSubTaskStatusAsync(WorkspaceId, SpaceId, TaskId, Id, User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }




    [HttpDelete("{Id}")]
    [HasPermission(Permissions.UpdateTasks)]
    public async Task<IActionResult> DeleteSubTask([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string TaskId, [FromRoute] string Id, CancellationToken cancellationToken = default!)
    {
        var result = await _SubTaskService.DeleteSubTaskAsync(WorkspaceId, SpaceId, TaskId, Id, User.GetUserId()!, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }



}
