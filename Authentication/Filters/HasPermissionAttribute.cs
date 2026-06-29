using AI_genda_API.Abstractions.Const;

namespace AI_genda_API.Authentication.Filters;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "Permission:";

    private static readonly HashSet<string> ValidPermissions =
        Permissions.GetAllPerimision.ToHashSet(StringComparer.OrdinalIgnoreCase);

    public HasPermissionAttribute(string permission)
    {
        if (!ValidPermissions.Contains(permission))
            throw new ArgumentOutOfRangeException(nameof(permission), $"Invalid permission '{permission}'.");

        Policy = $"{PolicyPrefix}{permission}";
    }
}