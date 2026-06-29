using AI_genda_API.Abstractions.Const;

namespace AI_genda_API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Admin)]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _RoleService = roleService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool hasincluded , CancellationToken cancellationToken)
    {
       var result = await _RoleService.GetAllAsync(cancellationToken , hasincluded);

        return Ok(result.Value);
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> Get([FromRoute] string Id )
    {
        var result = await _RoleService.GetAsync(Id);

        return result.IsSuccess ? Ok(result.Value): result.ToProblem();
    }

    [HttpPost()]
    public async Task<IActionResult> Add(RoleRequest request , CancellationToken cancellationToken)
    {
        var result = await _RoleService.AddAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value): result.ToProblem();
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update([FromRoute]string Id , RoleRequest request , CancellationToken cancellationToken)
    {
        var result = await _RoleService.UpdateAsync(Id , request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{Id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus([FromRoute] string Id)
    {
        var result = await _RoleService.ToggleStatusAsync(Id);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }




}
