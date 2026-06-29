namespace AI_genda_API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize( Roles = DefaultRoles.Member)]


public class WorkSpacesController(IWorkSpaceService workSpaceService) : ControllerBase
{
    private readonly IWorkSpaceService _WorkSpaceService = workSpaceService;


  
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] WorkSpaceRequest Requset, CancellationToken cancellationToken = default!)
    {
        var result = await _WorkSpaceService.AddAsync(User.GetUserId()!, Requset, cancellationToken);

        return result.IsSuccess ? CreatedAtAction(nameof(GetById),new { Id = result.Value.Id }, result.Value) : result.ToProblem();          
    }

    [HttpPost("{Id}/member")]
    [HasPermission(Permissions.AddWorkSpaces)]
    public async Task<IActionResult> AddMember([FromRoute] int Id, InviteMember request, CancellationToken cancellationToken = default!)
    {
        var res = await _WorkSpaceService.AddMemberAsync(Id, User.GetUserId()!, request, cancellationToken);

        return res.IsSuccess ? NoContent() : res.ToProblem();
    }


    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FilterRequest filterRequest, CancellationToken cancellationToken = default!)
    {
        var result = await _WorkSpaceService.GetAllAsync(filterRequest, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpGet("{Id}")]
    [HasPermission(Permissions.GetWorkSpaces)]
    public async Task<IActionResult> GetById([FromRoute]int id , CancellationToken CancelationToken)
    {
        var userId = User.GetUserId();

        var result = await _WorkSpaceService.GetByIdAsync(id, userId, CancelationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();


    }

  

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetWorkspaceDashboardData( CancellationToken cancellationToken = default!)
    {
        var result = await _WorkSpaceService.GetWorkspaceDashboardAsync(User.GetUserId()!, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpGet("{Id}/members")]
    [HasPermission(Permissions.GetWorkSpaces)]
    public async Task<IActionResult> GetMembers([FromRoute] int Id, CancellationToken cancellationToken = default!)
    {
        var result = await _WorkSpaceService.GetMembersAsync(Id, User.GetUserId()!, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }



    [HttpGet("{Id}/members/{MemberUserId}/permissions")]
    [HasPermission(Permissions.GetWorkSpaces)]
    public async Task<IActionResult> GetMemberPermissions([FromRoute] int Id, [FromRoute] string MemberUserId, CancellationToken cancellationToken = default!)
    {
        var result = await _WorkSpaceService.GetMemberPermissionsAsync(Id, User.GetUserId()!, MemberUserId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{Id}")]
    [HasPermission(Permissions.UpdateWorkSpaces)]
    public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] WorkSpaceRequest rquset,
        CancellationToken cancellationToken = default!)
    {
        var result = await _WorkSpaceService.UpdateAsync( Id ,User.GetUserId()!,rquset, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{Id}/restore")]
    [HasPermission(Permissions.DeleteWorkSpaces)]
    public async Task<IActionResult> Restore([FromRoute] int Id, CancellationToken cancellationToken = default!)
    {
        var result = await _WorkSpaceService.RestoreAsync(Id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }


    [HttpPut("{Id}/members/{MemberUserId}/permissions")]
    [HasPermission(Permissions.UpdateWorkSpaces)]
    public async Task<IActionResult> UpdateMemberPermissions([FromRoute] int Id, [FromRoute] string MemberUserId, [FromBody] UpdateWorkspaceMemberPermissionsRequest request, CancellationToken cancellationToken = default!)
    {
        var result = await _WorkSpaceService.UpdateMemberPermissionsAsync(Id, User.GetUserId()!, MemberUserId, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpDelete("{Id:int}")]
    [HasPermission(Permissions.DeleteWorkSpaces)]
    public async Task<IActionResult> Delete([FromRoute] int Id, CancellationToken cancellationToken = default!)
    {
        var res = await _WorkSpaceService.DeleteAsync(Id, cancellationToken);

        return res.IsSuccess ? NoContent() : res.ToProblem();
    }

  

    [HttpDelete("{Id:int}/remove-member")]
    public async Task<IActionResult> RemoveMember([FromRoute] int Id, InviteMember request, CancellationToken cancellationToken = default!)
    {
        var res = await _WorkSpaceService.RemoveMemberAsync( Id , User.GetUserId()!, request, cancellationToken);

        return res.IsSuccess ? NoContent() : res.ToProblem();
    }



    [HttpGet("deleted")]
    public async Task<IActionResult> GetAllDeleted(CancellationToken cancellationToken = default!)
    {
        var result = await _WorkSpaceService.GetAllDeletedAsync(User.GetUserId()!, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
  
  


    [HttpGet("{Id:int}/dashboard")]
    [HasPermission(Permissions.GetWorkSpaces)]
    public async Task<IActionResult> GetWorkspaceDashboardById([FromRoute] int Id, CancellationToken cancellationToken = default!)
    {
        var result = await _WorkSpaceService.GetWorkspaceDashboardByIdAsync(Id, User.GetUserId()!, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}
