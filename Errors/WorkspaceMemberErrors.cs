namespace AI_genda_API.Errors;

public class WorkspaceMemberErrors
{
    public static Error MemberNotFounded =
        new Error("WorkSpaceMember.NotAmember", "This user is not a member on this WorkSapce", StatusCodes.Status404NotFound);

    public static Error AccessDenied =
        new Error("WorkspaceMember.AccessDenied", "You don't have permission to do actions on this workspace.", StatusCodes.Status401Unauthorized);

    public static Error InvalidPermissions =
        new Error("WorkspaceMember.InvalidPermissions", "One or more provided permissions are invalid for workspace members.", StatusCodes.Status400BadRequest);

    public static Error OwnerPermissionsImmutable =
        new Error("WorkspaceMember.OwnerPermissionsImmutable", "Owner permissions cannot be changed .", StatusCodes.Status400BadRequest);

    public static Error InvalidAssign =
        new Error("WorkspaceMember.InvalidAssign", "You can't assign members to private workspace", StatusCodes.Status400BadRequest);



}
