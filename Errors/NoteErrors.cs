namespace AI_genda_API.Errors;

public static class NoteErrors
{
    public static readonly Error NoteNotFound =
        new("Note.NotFound", "Note not found or has been deleted", StatusCodes.Status404NotFound);

    public static readonly Error VoiceAssetRequired =
        new("Note.VoiceAssetRequired", "Voice note must contain at least one audio asset.", StatusCodes.Status400BadRequest);

    public static readonly Error ImageAssetRequired =
        new("Note.ImageAssetRequired", "Image note must contain at least one image asset.", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidAssetForType =
        new("Note.InvalidAssetForType", "Provided assets do not match the selected note type.", StatusCodes.Status400BadRequest);

    public static readonly Error UnsupportedMediaFormat =
        new("Note.UnsupportedMediaFormat", "Unsupported media format.", StatusCodes.Status400BadRequest);

    public static readonly Error AssetTooLarge =
        new("Note.AssetTooLarge", "Media file exceeds maximum allowed size.", StatusCodes.Status400BadRequest);
}