namespace AI_genda_API.Errors;

public class WorkSpaceErrors
{
    public static Error WorkSpaceNotFound => 
        new Error("WorkSpace.NotFound", "This workspace is not existed", StatusCodes.Status404NotFound); 
    public static Error TargetWorkSpaceNotFound => 
        new Error("WorkSpace.NotFound", "This target workspace is not existed", StatusCodes.Status404NotFound); 
}
