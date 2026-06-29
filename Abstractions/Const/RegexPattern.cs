namespace AI_genda_API.Abstractions.Const;

public static class RegexPattern
{
    public const string Password =
       "^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9_]).{8,}$";

}
