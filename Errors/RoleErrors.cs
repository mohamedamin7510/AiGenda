namespace BucketSurvey.Api.Errors;

public class RoleErrors
{
    public static readonly Error RoleNotFound =
        new("Role.NotFound", "The specified role was not founded in the roles." , StatusCodes.Status404NotFound);
    
    public static readonly Error RoleDuplicated =
        new("Role.Duplicated", "You can't insert duplicted role " , StatusCodes.Status409Conflict);

    public static readonly Error InvalidRole=
        new("Role.Invalid", "Invalid Role!" , StatusCodes.Status400BadRequest);
    

}
