using AI_genda_API.Abstractions.Enums;

namespace AI_genda_API.Contracts.Note;

public record NoteRequest
{
    public string Title { get; set; } = string.Empty;
    public NoteType Type { get; set; }
    public bool IsPinned { get; set; }
    public TextPayload? Text { get; set; }
    public VoicePayload? Voice { get; set; }
    public ImagePayload? Image { get; set; }
    public HandDrawPayload? HandDraw { get; set; }
    public List<AssetPayload>? Assets { get; set; }
}