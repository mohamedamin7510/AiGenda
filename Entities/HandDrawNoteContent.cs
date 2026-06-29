using System.ComponentModel.DataAnnotations.Schema;

namespace AI_genda_API.Entities;

[Table("HandDrawNoteContents")]
public class HandDrawNoteContent
{
    [ForeignKey(nameof(Note))]
    public string NoteId { get; set; } = string.Empty;

    public string DrawingJson { get; set; } = string.Empty;
    public string? ExtractedText { get; set; }

    public virtual Note Note { get; set; } = null!;
}