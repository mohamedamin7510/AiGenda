using System.ComponentModel.DataAnnotations.Schema;

namespace AI_genda_API.Entities;

[Table("TextNoteContents")]
public class TextNoteContent
{
    [ForeignKey(nameof(Note))]
    public string NoteId { get; set; } = string.Empty;

    public string PlainText { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
    public string? RichTextJson { get; set; }

    public virtual Note Note { get; set; } = null!;
}