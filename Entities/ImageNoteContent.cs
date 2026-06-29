using System.ComponentModel.DataAnnotations.Schema;

namespace AI_genda_API.Entities;

[Table("ImageNoteContents")]
public class ImageNoteContent
{
    [ForeignKey(nameof(Note))]
    public string NoteId { get; set; } = string.Empty;

    public string? OcrText { get; set; }
    public string? Caption { get; set; }

    public virtual Note Note { get; set; } = null!;
}