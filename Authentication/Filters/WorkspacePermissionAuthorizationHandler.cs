using Microsoft.AspNetCore.Mvc.Controllers;

namespace AI_genda_API.Authentication.Filters;

public sealed class WorkspacePermissionAuthorizationHandler(AppContext context, IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<PermissionRequirement>
{
    private readonly AppContext _Context = context;
    private readonly IHttpContextAccessor _HttpContextAccessor = httpContextAccessor;

    protected override async System.Threading.Tasks.Task HandleRequirementAsync(AuthorizationHandlerContext authContext, PermissionRequirement requirement)
    {
        var httpContext = _HttpContextAccessor.HttpContext;

        if (httpContext is null)
            return;

        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return;

        // Optional guard: ensure permission family matches controller
        if (!IsPermissionCompatibleWithEndpoint(httpContext, requirement.Permission))
            return;

        if (!TryResolveWorkspaceId(httpContext, out var workspaceId))
            return;

        var workspace = await _Context.WorkSpaces
            .Where(w => w.Id == workspaceId && w.RemovedAt == null)
            .Select(w => new { w.Id, w.CreatedById })
            .SingleOrDefaultAsync();

        if (workspace is null)
            return;

        // Owner bypass
        if (workspace.CreatedById == userId)
        {
            authContext.Succeed(requirement);
            return;
        }

        var memberPermissions = await _Context.WorkspaceMembers
            .Where(m => m.WrokSpaceID == workspaceId && m.UserID == userId)
            .Select(m => m.Permissions)
            .SingleOrDefaultAsync();

        if (memberPermissions is null)
            return;

        if (memberPermissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
            authContext.Succeed(requirement);
    }

    private static bool TryResolveWorkspaceId(HttpContext httpContext, out int workspaceId)
    {
        workspaceId = 0;
        var rv = httpContext.Request.RouteValues;

        if (rv.TryGetValue("WorkspaceId", out var wsObj) && int.TryParse(wsObj?.ToString(), out workspaceId))
            return true;

        if (rv.TryGetValue("Id", out var idObj) && int.TryParse(idObj?.ToString(), out workspaceId))
            return true;

        return false;
    }

    private static bool IsPermissionCompatibleWithEndpoint(HttpContext httpContext, string permission)
    {
        var action = httpContext.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();

        var controller = action?.ControllerName;

        if (string.IsNullOrWhiteSpace(controller))
            return true;

        return controller switch
        {
            "WorkSpaces" => permission.StartsWith("workspaces:", StringComparison.OrdinalIgnoreCase) ||
                            permission.StartsWith("users:", StringComparison.OrdinalIgnoreCase),
            "Spaces" => permission.StartsWith("spaces:", StringComparison.OrdinalIgnoreCase),
            "Tasks" => permission.StartsWith("tasks:", StringComparison.OrdinalIgnoreCase),
            _ => true
        };
    }


}