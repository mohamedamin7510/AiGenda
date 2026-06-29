using AI_genda_API.Abstractions.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_genda_API.Entities;

[Table("Notes")]
public class Note : AuditLogging
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SpaceId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public NoteType Type { get; set; } = NoteType.Text;
    public bool IsActive { get; set; } = true;
    public bool IsPinned { get; set; }

    [ForeignKey(nameof(SpaceId))]
    public virtual Space Space { get; set; } = null!;

    public virtual TextNoteContent? TextContent { get; set; }
    public virtual VoiceNoteContent? VoiceContent { get; set; }
    public virtual ImageNoteContent? ImageContent { get; set; }
    public virtual HandDrawNoteContent? HandDrawContent { get; set; }

    public virtual List<NoteAsset> Assets { get; set; } = [];
}