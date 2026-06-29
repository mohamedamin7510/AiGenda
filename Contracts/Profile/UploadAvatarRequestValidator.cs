namespace AI_genda_API.Contracts.Profile;

public class UploadAvatarRequestValidator : AbstractValidator<UploadAvatarRequest>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedContentTypes = ["image/jpg", "image/jpeg", "image/png", "image/webp"];
    private const long MaxSizeBytes = 2 * 1024 * 1024; 

    public UploadAvatarRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("Avatar image is required.");


        RuleFor(x => x.File.Length)
            .GreaterThan(0)
            .WithMessage("Avatar image cannot be empty.")
            .LessThanOrEqualTo(MaxSizeBytes)
            .WithMessage("Avatar size must be less than or equal to 2 MB.");


        RuleFor(x => x.File.FileName)
            .Must(fileName =>
            {
                var ext = Path.GetExtension(fileName).ToLowerInvariant();

                return AllowedExtensions.Contains(ext);
            })
            .WithMessage("Allowed file types: .jpg, .jpeg, .png, .webp.");


        RuleFor(x => x.File.ContentType)
            .Must(contentType => AllowedContentTypes.Contains(contentType.ToLowerInvariant()))
            .WithMessage("Invalid image content type.");

    }
}
