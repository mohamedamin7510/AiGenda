using AI_genda_API.Abstractions.Enums;

namespace AI_genda_API.Contracts.Note;

public class NoteRequestValidator : AbstractValidator<NoteRequest>
{
    private const long MaxAudioBytes = 25 * 1024 * 1024;
    private const long MaxImageBytes = 10 * 1024 * 1024;

    private static readonly HashSet<string> AllowedAudioMime = ["audio/mpeg", "audio/wav", "audio/x-wav"];
    private static readonly HashSet<string> AllowedImageMime = ["image/jpeg", "image/png"];

    private static readonly HashSet<string> AllowedAudioExt = [".mp3", ".wav"];
    private static readonly HashSet<string> AllowedImageExt = [".jpg", ".jpeg", ".png"];

    public NoteRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x)
            .Must(HaveOnlySelectedPayload)
            .WithMessage("Request must contain only the payload for the selected note type.");

        RuleFor(x => x)
            .Must(HaveValidByTypeContent)
            .WithMessage("Payload does not match selected note type.");

        RuleFor(x => x)
            .Must(HaveValidAssetFormatsAndSizes)
            .WithMessage("One or more assets have invalid format or size.");
    }

    private static bool HaveOnlySelectedPayload(NoteRequest request)
    {
        return request.Type switch
        {
            NoteType.Text =>
                request.Text is not null &&
                request.Voice is null &&
                request.Image is null &&
                request.HandDraw is null,

            NoteType.Voice =>
                request.Text is null &&
                request.Image is null &&
                request.HandDraw is null,

            NoteType.Image =>
                request.Text is null &&
                request.Voice is null &&
                request.HandDraw is null,

            NoteType.HandDraw =>
                request.HandDraw is not null &&
                request.Text is null &&
                request.Voice is null &&
                request.Image is null,

            _ => false
        };
    }

    private static bool HaveValidByTypeContent(NoteRequest request)
    {
        return request.Type switch
        {
            NoteType.Text =>
                request.Text is not null &&
                !string.IsNullOrWhiteSpace(request.Text.HtmlContent),

            NoteType.Voice =>
                request.Assets?.Any(a => a.AssetType == NoteAssetType.Audio && a.File is not null) == true,

            NoteType.Image =>
                request.Assets?.Any(a => a.AssetType == NoteAssetType.Image && a.File is not null) == true,

            NoteType.HandDraw =>
                request.HandDraw is not null &&
                !string.IsNullOrWhiteSpace(request.HandDraw.DrawingJson),

            _ => false
        };
    }

    private static bool HaveValidAssetFormatsAndSizes(NoteRequest request)
    {
        if (request.Assets is null || request.Assets.Count == 0)
            return request.Type is NoteType.Text or NoteType.HandDraw;

        foreach (var asset in request.Assets)
        {
            if (asset.File is null || asset.File.Length <= 0)
                return false;

            var ext = Path.GetExtension(asset.File.FileName).ToLowerInvariant();
            var mime = asset.File.ContentType.ToLowerInvariant();
            var size = asset.File.Length;

            if (request.Type == NoteType.Voice)
            {
                if (asset.AssetType != NoteAssetType.Audio)
                    return false;

                if (!AllowedAudioExt.Contains(ext) || !AllowedAudioMime.Contains(mime))
                    return false;

                if (size > MaxAudioBytes)
                    return false;
            }
            else if (request.Type == NoteType.Image)
            {
                if (asset.AssetType != NoteAssetType.Image)
                    return false;

                if (!AllowedImageExt.Contains(ext) || !AllowedImageMime.Contains(mime))
                    return false;

                if (size > MaxImageBytes)
                    return false;
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}