namespace AI_genda_API.Errors;

public static class SpaceErrors
{
    public static readonly Error SpaceNotFound = new("Space.NotFound", "Space not founded or has been deleted.", StatusCodes.Status404NotFound);
}
