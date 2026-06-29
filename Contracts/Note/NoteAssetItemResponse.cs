using AI_genda_API.Abstractions.Enums;

namespace AI_genda_API.Contracts.Note;

public record NoteAssetItemResponse
(
     string Id,
     NoteAssetType AssetType,
     string FileName,
     string StorageUrl,
     string MimeType,
     long SizeInBytes,
     int? DurationSeconds
);
