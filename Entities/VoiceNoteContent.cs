using System.ComponentModel.DataAnnotations.Schema;

namespace AI_genda_API.Entities;

[Table("VoiceNoteContents")]
public class VoiceNoteContent
{
    [ForeignKey(nameof(Note))]
    public string NoteId { get; set; } = string.Empty;

    public string? TranscriptText { get; set; }

    public virtual Note Note { get; set; } = null!;
}