using AI_genda_API.Abstractions.Enums;

namespace AI_genda_API.Contracts.Note;

public class AssetPayload
{
    public NoteAssetType AssetType { get; set; }
    public IFormFile File { get; set; } = default!;
    public int? DurationSeconds { get; set; }
}

