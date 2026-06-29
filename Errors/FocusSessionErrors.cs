namespace AI_genda_API.Errors;

public static class FocusSessionErrors
{
    public static readonly Error SessionNotFound =
        new("FocusSession.NotFound", "Focus session was not found.", StatusCodes.Status404NotFound);

    public static readonly Error ActiveSessionAlreadyExists =
        new("FocusSession.ActiveAlreadyExists", "Active or paused session already exists for this task.", StatusCodes.Status409Conflict);

    public static readonly Error SessionNotActive =
        new("FocusSession.NotActive", "Session must be active.", StatusCodes.Status400BadRequest);

    public static readonly Error SessionNotPaused =
        new("FocusSession.NotPaused", "Session must be paused.", StatusCodes.Status400BadRequest);

    public static readonly Error SessionClosed =
        new("FocusSession.Closed", "Session is already completed or abandoned.", StatusCodes.Status400BadRequest);

    public static readonly Error NoCurrentSession =
        new("FocusSession.NoCurrent", "No active or paused session found.", StatusCodes.Status404NotFound);
}