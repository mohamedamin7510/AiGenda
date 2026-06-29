namespace AI_genda_API.Errors;

public static class SubTaskErrors
{
    public static readonly Error SubTaskNotFound =
        new("SubTask.NotFound", "Subtask not founded or has been deleted.", StatusCodes.Status404NotFound);
}