namespace AI_genda_API.Extenstions;

public static class TokenizeExtentstion
{
    public static string[] Tokenize(this string? search)
    {
        return string.IsNullOrWhiteSpace(search) ? [] :
        search.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }



}
