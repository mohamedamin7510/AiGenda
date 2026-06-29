namespace AI_genda_API.Controllers;

[ApiController]
[Route("api/WorkSpaces/{WorkspaceId:int}/Spaces/{SpaceId}/[controller]")]
[Authorize(Roles = DefaultRoles.Member)]
public class NotesController(INoteService noteService) : ControllerBase
{
    private readonly INoteService _NoteService = noteService;

    [HttpPost]
    [Consumes("multipart/form-data")]
    [HasPermission(Permissions.AddNotes)]
    public async Task<IActionResult> Add([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromForm] NoteRequest request, CancellationToken cancellationToken = default!)
    {
        var result = await _NoteService.AddAsync(WorkspaceId, SpaceId, User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { WorkspaceId, SpaceId, Id = result.Value.Id }, result.Value) : result.ToProblem();
    }


    [HttpGet]
    [HasPermission(Permissions.GetNotes)]
    public async Task<IActionResult> GetAll([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromQuery] FilterRequest filterRequest, CancellationToken cancellationToken = default!)
    {
        var result = await _NoteService.GetAllAsync(WorkspaceId, SpaceId, User.GetUserId()!, filterRequest, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpGet("{Id}")]
    [HasPermission(Permissions.GetNotes)]
    public async Task<IActionResult> GetById([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string Id, CancellationToken cancellationToken = default!)
    {
        var result = await _NoteService.GetByIdAsync(WorkspaceId, SpaceId, Id, User.GetUserId()!, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpPut("{Id}")]
    [Consumes("multipart/form-data")]
    [HasPermission(Permissions.UpdateNotes)]
    public async Task<IActionResult> Update([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string Id, [FromForm] NoteRequest request, CancellationToken cancellationToken = default!)
    {
        var result = await _NoteService.UpdateAsync(WorkspaceId, SpaceId, Id, User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpDelete("{Id}")]
    [HasPermission(Permissions.DeleteNotes)]
    public async Task<IActionResult> Delete([FromRoute] int WorkspaceId, [FromRoute] string SpaceId, [FromRoute] string Id, CancellationToken cancellationToken = default!)
    {
        var result = await _NoteService.DeleteAsync(WorkspaceId, SpaceId, Id, User.GetUserId()!, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
