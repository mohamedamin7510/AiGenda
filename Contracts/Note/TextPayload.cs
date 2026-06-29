namespace AI_genda_API.Contracts.Note;

public record TextPayload
(
   string PlainText,
   string HtmlContent,
   string? RichTextJson
);



