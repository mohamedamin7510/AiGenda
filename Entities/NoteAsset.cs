using AI_genda_API.Abstractions.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_genda_API.Entities;

[Table("NoteAssets")]
public class NoteAsset
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string NoteId { get; set; } = string.Empty;
    public NoteAssetType AssetType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public int? DurationSeconds { get; set; }




    [ForeignKey(nameof(NoteId))]
    public virtual Note Note { get; set; } = null!;
}