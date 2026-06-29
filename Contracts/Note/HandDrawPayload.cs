namespace AI_genda_API.Contracts.Note;

public record HandDrawPayload
(
    string DrawingJson, 
    string? ExtractedText
);

