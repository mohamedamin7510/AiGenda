using System.Security.Claims;

namespace BucketSurvey.Api.Extenstions;

public static class ExtenstionUserId
{
    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal) =>
         claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

}
