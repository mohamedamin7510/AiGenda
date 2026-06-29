 using AI_genda_API.Contracts.Results;

namespace AI_genda_API.Controllers;

[ApiController]
[Route("api/WorkSpaces/{WorkspaceId:int}/[controller]")]
[Authorize(Roles = DefaultRoles.Member)]
public class SpacesController(ISpaceService spaceService) : ControllerBase
{
    private readonly ISpaceService _SpaceService = spaceService;

    [HttpPost]
    [HasPermission(Permissions.AddSpaces)]
    public async Task<IActionResult> Add([FromRoute] int WorkspaceId, [FromBody] SpaceRequest request, CancellationToken cancellationToken = default!)
    {
        var result = await _SpaceService.AddAsync(WorkspaceId, User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ?
            CreatedAtAction(nameof(GetById), new { WorkspaceId, Id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpGet]
    [HasPermission(Permissions.GetSpaces)]
    public async Task<IActionResult> GetAll([FromRoute] int WorkspaceId, [FromQuery] FilterRequest filterRequest, CancellationToken cancellationToken = default!)
    {
        var result = await _SpaceService.GetAllAsync(WorkspaceId, User.GetUserId()!, filterRequest, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("deleted")]
    [HasPermission(Permissions.DeleteSpaces)]
    public async Task<IActionResult> GetAllDeleted([FromRoute] int WorkspaceId, CancellationToken cancellationToken = default!)
    {
        var result = await _SpaceService.GetAllDeletedAsync(WorkspaceId, User.GetUserId()!, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{Id}")]
    [HasPermission(Permissions.GetSpaces)]
    public async Task<IActionResult> GetById([FromRoute] int WorkspaceId, [FromRoute] string Id, CancellationToken cancellationToken = default!)
    {
        var result = await _SpaceService.GetByIdAsync(WorkspaceId, Id, User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

   
    [HttpPut("{Id}")]
    [HasPermission(Permissions.UpdateSpaces)]
    public async Task<IActionResult> Update([FromRoute] int WorkspaceId, [FromRoute] string Id, [FromBody] SpaceRequest request, CancellationToken cancellationToken = default!)
    {
        var result = await _SpaceService.UpdateAsync(WorkspaceId, Id, User.GetUserId()!, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{Id}/move")]
    [HasPermission(Permissions.UpdateSpaces)]
    public async Task<IActionResult> Move([FromRoute] int WorkspaceId, [FromRoute] string Id, [FromBody] MoveSpaceRequest request, CancellationToken cancellationToken = default!)
    {
        var result = await _SpaceService.MoveAsync(WorkspaceId, Id, User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{Id}")]
    [HasPermission(Permissions.DeleteSpaces)]
    public async Task<IActionResult> Delete([FromRoute] int WorkspaceId, [FromRoute] string Id, CancellationToken cancellationToken = default!)
    {
        var result = await _SpaceService.DeleteAsync(WorkspaceId, Id, User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{Id}/restore")]
    [HasPermission(Permissions.DeleteSpaces)]
    public async Task<IActionResult> Restore([FromRoute] int WorkspaceId, [FromRoute] string Id, CancellationToken cancellationToken = default!)
    {
        var result = await _SpaceService.RestoreAsync(WorkspaceId, Id, User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }


    [HttpGet("{Id}/analytics")]
    public async Task<IActionResult> GetResults([FromRoute] int WorkspaceId,  [FromRoute] string Id,  [FromQuery] SpaceAnalyticsQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _SpaceService.GetResultsAsync(WorkspaceId, Id, User.GetUserId()!, request, cancellationToken);

        return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
    }


    [HttpGet("{Id}/analytics/export")]
    public async Task<IActionResult> ExportResults([FromRoute] int WorkspaceId, [FromRoute] string Id ,[FromQuery] SpaceAnalyticsQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _SpaceService.ExportResultsAsync(WorkspaceId, Id, User.GetUserId()!, request, cancellationToken);

        return response.IsSuccess ? File(response.Value.Content, response.Value.ContentType, response.Value.FileName) : response.ToProblem();
    }
     




}