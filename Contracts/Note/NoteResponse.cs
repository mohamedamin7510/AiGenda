using AI_genda_API.Abstractions.Enums;

namespace AI_genda_API.Contracts.Note;

public record NoteResponse(
    string Id,
    string SpaceId,
    string Title,
    NoteType Type,
    bool IsPinned,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? PlainText,
    string? HtmlContent,
    string? RichTextJson,
    string? TranscriptText,
    string? OcrText,
    string? Caption,
    string? DrawingJson,
    string? ExtractedText,
    List<NoteAssetItemResponse> Assets);
 
