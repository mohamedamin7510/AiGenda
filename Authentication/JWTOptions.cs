using System.Diagnostics.CodeAnalysis;

namespace AI_genda_API.Authentication;

public class JWTOptions
{
    public static string SectionName { get; set; } = "Jwt";
    [NotNull]
    public string SymmetricKey { get; init; } = default!;
    [NotNull]
    public string Issuer { get; init; } = default!;
    [NotNull]
    public string Audience { get; init; } = default!;

    [Range(10, 40)]
    public int ExpirtMiniuites { get; init; } = default;

}
