using AI_genda_API.Abstractions.Enums;
using AI_genda_API.Contracts.Common;
using AI_genda_API.Contracts.Note;

namespace AI_genda_API.Services.NoteService;

public class NoteService(AppContext context, IWebHostEnvironment webHostEnvironment) : INoteService
{
    private readonly AppContext _Context = context;
    private readonly IWebHostEnvironment _WebHostEnvironment = webHostEnvironment;

    private const long MaxAudioBytes = 25 * 1024 * 1024;
    private const long MaxImageBytes = 10 * 1024 * 1024;

    private static readonly HashSet<string> AllowedAudioMime = ["audio/mpeg", "audio/wav", "audio/x-wav"];
    private static readonly HashSet<string> AllowedImageMime = ["image/jpeg", "image/png"];
    private static readonly HashSet<string> AllowedAudioExt = [".mp3", ".wav"];
    private static readonly HashSet<string> AllowedImageExt = [".jpg", ".jpeg", ".png"];





    public async Task<Result<NoteResponse>> AddAsync(int WorkspaceId, string SpaceId, string UserId, NoteRequest request, CancellationToken cancellationToken = default!)
    {

        if (!await HasAccessAsync(WorkspaceId, UserId, cancellationToken))
            return Result.Faluire<NoteResponse>(WorkspaceMemberErrors.AccessDenied);

        var spaceExists = await _Context.Spaces
            .AnyAsync(s => s.Id == SpaceId && s.WorkSpaceId == WorkspaceId && s.IsActive && s.RemovedAt == null, cancellationToken);

        if (!spaceExists)
            return Result.Faluire<NoteResponse>(SpaceErrors.SpaceNotFound);

        var assetValidationError = ValidateAssetsForType(request);
        if (assetValidationError is not null)
            return Result.Faluire<NoteResponse>(assetValidationError);

        var note = new Note
        {
            SpaceId = SpaceId,
            Title = request.Title,
            Type = request.Type,
            IsPinned = request.IsPinned,
            Assets = await BuildAssetsAsync(request.Assets, request.Type, cancellationToken)
        };

        ApplyTypedContent(note, request);

        await _Context.Notes.AddAsync(note, cancellationToken);
        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(note));
    }

    public async Task<Result<PaginatedList<NoteResponse>>> GetAllAsync(int WorkspaceId, string SpaceId, string UserId, FilterRequest filterRequest, CancellationToken cancellationToken = default!)
    {

        if (!await HasAccessAsync(WorkspaceId, UserId, cancellationToken))
            return Result.Faluire<PaginatedList<NoteResponse>>(WorkspaceMemberErrors.AccessDenied);

        var spaceExists = await _Context.Spaces
            .AnyAsync(s => s.Id == SpaceId && s.WorkSpaceId == WorkspaceId && s.IsActive && s.RemovedAt == null, cancellationToken);

        if (!spaceExists)
            return Result.Faluire<PaginatedList<NoteResponse>>(SpaceErrors.SpaceNotFound);

        var search = filterRequest.SearchValue?.Trim();

        IQueryable<Note> query = _Context.Notes
            .Where(n => n.SpaceId == SpaceId && n.RemovedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";

            query = query.Where(n =>
                EF.Functions.Like(n.Title, pattern) ||
                (n.TextContent != null && EF.Functions.Like(n.TextContent.PlainText, pattern)) ||
                (n.VoiceContent != null && n.VoiceContent.TranscriptText != null && EF.Functions.Like(n.VoiceContent.TranscriptText, pattern)) ||
                (n.ImageContent != null && n.ImageContent.OcrText != null && EF.Functions.Like(n.ImageContent.OcrText, pattern)) ||
                (n.HandDrawContent != null && n.HandDrawContent.ExtractedText != null && EF.Functions.Like(n.HandDrawContent.ExtractedText, pattern)));
        }

        var ordered = query.OrderBy(BuildOrderBy(filterRequest));

        var responseQuery = ordered
            .Select(n => new NoteResponse(
                n.Id,
                n.SpaceId,
                n.Title,
                n.Type,
                n.IsPinned,
                n.CreatedAt,
                n.UpdatedAt,
                n.TextContent != null ? n.TextContent.PlainText : null,
                n.TextContent != null ? n.TextContent.HtmlContent : null,
                n.TextContent != null ? n.TextContent.RichTextJson : null,
                n.VoiceContent != null ? n.VoiceContent.TranscriptText : null,
                n.ImageContent != null ? n.ImageContent.OcrText : null,
                n.ImageContent != null ? n.ImageContent.Caption : null,
                n.HandDrawContent != null ? n.HandDrawContent.DrawingJson : null,
                n.HandDrawContent != null ? n.HandDrawContent.ExtractedText : null,
                n.Assets.Select(a => new NoteAssetItemResponse(
                    a.Id,
                    a.AssetType,
                    a.FileName,
                    a.StorageUrl,
                    a.MimeType,
                    a.SizeInBytes,
                    a.DurationSeconds
                )).ToList()
            ))
            .AsNoTracking();


        var paged = await PaginatedList<NoteResponse>.CreateAsync( responseQuery, filterRequest.PageNumber,filterRequest.PageSize) ;

        return Result.Success(paged);
    }

    public async Task<Result<NoteResponse>> GetByIdAsync(int WorkspaceId, string SpaceId, string Id, string UserId, CancellationToken cancellationToken = default!)
    {
        if (!await HasAccessAsync(WorkspaceId, UserId, cancellationToken))
            return Result.Faluire<NoteResponse>(WorkspaceMemberErrors.AccessDenied);

        var note = await _Context.Notes
            .Include(n => n.TextContent)
            .Include(n => n.VoiceContent)
            .Include(n => n.ImageContent)
            .Include(n => n.HandDrawContent)
            .Include(n => n.Assets)
            .SingleOrDefaultAsync(n => n.Id == Id && n.SpaceId == SpaceId && n.RemovedAt == null, cancellationToken);

        if (note is null)
            return Result.Faluire<NoteResponse>(NoteErrors.NoteNotFound);

        return Result.Success(MapToResponse(note));
    }

    public async Task<Result<NoteResponse>> UpdateAsync(int WorkspaceId, string SpaceId, string Id, string UserId, NoteRequest request, CancellationToken cancellationToken = default!)
    {

        if (!await HasAccessAsync(WorkspaceId, UserId, cancellationToken))
            return Result.Faluire<NoteResponse>(WorkspaceMemberErrors.AccessDenied);

        var note = await _Context.Notes
            .Include(n => n.TextContent)
            .Include(n => n.VoiceContent)
            .Include(n => n.ImageContent)
            .Include(n => n.HandDrawContent)
            .Include(n => n.Assets)
            .SingleOrDefaultAsync(n => n.Id == Id && n.SpaceId == SpaceId && n.Type == request.Type && n.RemovedAt == null, cancellationToken);

        if (note is null)
            return Result.Faluire<NoteResponse>(NoteErrors.NoteNotFound);


        DeleteLocalFiles(note.Assets);

        note.Title = request.Title;
        note.Type = request.Type;
        note.IsPinned = request.IsPinned;

        RemoveTypedContent(note);

        ApplyTypedContent(note, request);

        _Context.NoteAssets.RemoveRange(note.Assets);

        note.Assets = await BuildAssetsAsync(request.Assets, request.Type, cancellationToken);

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(note));
    }

    public async Task<Result> DeleteAsync(int WorkspaceId, string SpaceId, string Id, string UserId, CancellationToken cancellationToken = default!)
    {


        if (!await HasAccessAsync(WorkspaceId, UserId, cancellationToken))
            return Result.Faluire(WorkspaceMemberErrors.AccessDenied);

        var note = await _Context.Notes
            .Include(n => n.Assets)
            .SingleOrDefaultAsync(n => n.Id == Id && n.SpaceId == SpaceId && n.RemovedAt == null, cancellationToken);


        if (note is null)
            return Result.Faluire(NoteErrors.NoteNotFound);


        DeleteLocalFiles(note.Assets);

        note.IsActive = false;
        note.RemovedAt = DateTime.UtcNow;
        note.RemovedById = UserId;

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }










    private async Task<bool> HasAccessAsync(int WorkspaceId, string UserId, CancellationToken cancellationToken)
    {
        return await _Context.WorkSpaces.AnyAsync(w => w.Id == WorkspaceId && w.CreatedById == UserId, cancellationToken)
            || await _Context.WorkspaceMembers.AnyAsync(m => m.WrokSpaceID == WorkspaceId && m.UserID == UserId, cancellationToken);
    }

    private static Error? ValidateAssetsForType(NoteRequest request)
    {
        var assets = request.Assets ?? [];

        if (request.Type == NoteType.Voice)
        {
            if (assets.Count == 0 || assets.All(a => a.AssetType != NoteAssetType.Audio))
                return NoteErrors.VoiceAssetRequired;

            foreach (var asset in assets)
            {
                if (asset.File is null || asset.File.Length <= 0)
                    return NoteErrors.VoiceAssetRequired;

                if (asset.AssetType != NoteAssetType.Audio)
                    return NoteErrors.InvalidAssetForType;

                if (!IsSupportedAudio(asset))
                    return NoteErrors.UnsupportedMediaFormat;

                if (asset.File.Length > MaxAudioBytes)
                    return NoteErrors.AssetTooLarge;
            }
        }

        if (request.Type == NoteType.Image)
        {
            if (assets.Count == 0 || assets.All(a => a.AssetType != NoteAssetType.Image))
                return NoteErrors.ImageAssetRequired;

            foreach (var asset in assets)
            {
                if (asset.File is null || asset.File.Length <= 0)
                    return NoteErrors.ImageAssetRequired;

                if (asset.AssetType != NoteAssetType.Image)
                    return NoteErrors.InvalidAssetForType;

                if (!IsSupportedImage(asset))
                    return NoteErrors.UnsupportedMediaFormat;

                if (asset.File.Length > MaxImageBytes)
                    return NoteErrors.AssetTooLarge;
            }
        }

        if (request.Type is NoteType.Text or NoteType.HandDraw)
        {
            if (assets.Count > 0)
                return NoteErrors.InvalidAssetForType;
        }

        return null;
    }

    private static bool IsSupportedAudio(AssetPayload asset)
    {
        if (asset.File is null) return false;

        var ext = Path.GetExtension(asset.File.FileName).ToLowerInvariant();

        var mime = (asset.File.ContentType ?? string.Empty).ToLowerInvariant();

        return AllowedAudioExt.Contains(ext) && AllowedAudioMime.Contains(mime);
    }

    private static bool IsSupportedImage(AssetPayload asset)
    {
        if (asset.File is null) return false;

        var ext = Path.GetExtension(asset.File.FileName).ToLowerInvariant();
        var mime = (asset.File.ContentType ?? string.Empty).ToLowerInvariant();

        return AllowedImageExt.Contains(ext) && AllowedImageMime.Contains(mime);
    }

    private async Task<(string RelativeUrl, string OriginalFileName, string ContentType, long Size)> SaveAssetFileAsync(AssetPayload asset, NoteType noteType, CancellationToken cancellationToken)
    {
        if (asset.File is null)
            throw new InvalidOperationException("Asset file is required.");

        var ext = Path.GetExtension(asset.File.FileName).ToLowerInvariant();
        var root = _WebHostEnvironment.WebRootPath ?? Path.Combine(_WebHostEnvironment.ContentRootPath, "wwwroot");
        var folder = noteType == NoteType.Voice ? "voice" : "images";
        var fullFolder = Path.Combine(root, "uploads", "notes", folder);

        Directory.CreateDirectory(fullFolder);

        var newName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(fullFolder, newName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await asset.File.CopyToAsync(stream, cancellationToken);

        return ($"/uploads/notes/{folder}/{newName}", asset.File.FileName, asset.File.ContentType, asset.File.Length);
    }

    private void DeleteLocalFiles(IEnumerable<NoteAsset> assets)
    {
        var root = _WebHostEnvironment.WebRootPath ?? Path.Combine(_WebHostEnvironment.ContentRootPath, "wwwroot");

        foreach (var asset in assets)
        {

            if (string.IsNullOrWhiteSpace(asset.StorageUrl) || !asset.StorageUrl.StartsWith("/uploads/notes/", StringComparison.OrdinalIgnoreCase))
                continue;

            var relative = asset.StorageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);

            var fullPath = Path.Combine(root, relative);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }

    private static string BuildOrderBy(FilterRequest filterRequest)
    {
        var fallback = "CreatedAt desc";

        if (string.IsNullOrWhiteSpace(filterRequest.SortColumn))
            return fallback;

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["title"] = "Title",
            ["type"] = "Type",
            ["createdat"] = "CreatedAt",
            ["updatedat"] = "UpdatedAt",
            ["ispinned"] = "IsPinned"
        };

        if (!map.TryGetValue(filterRequest.SortColumn.Trim(), out var column))
            return fallback;

        var direction = string.Equals(filterRequest.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";

        return $"{column} {direction}";
    }

    private void RemoveTypedContent(Note note)
    {

        if (note.TextContent is not null) 
            _Context.TextNoteContents.Remove(note.TextContent);

        if (note.VoiceContent is not null) 
            _Context.VoiceNoteContents.Remove(note.VoiceContent);

        if (note.ImageContent is not null)
            _Context.ImageNoteContents.Remove(note.ImageContent);

        if (note.HandDrawContent is not null)
            _Context.HandDrawNoteContents.Remove(note.HandDrawContent);


        note.TextContent = null;
        note.VoiceContent = null;
        note.ImageContent = null;
        note.HandDrawContent = null;
    }

    private static void ApplyTypedContent(Note note, NoteRequest request)
    {

        switch (request.Type)
        {
            case NoteType.Text:
                note.TextContent = new TextNoteContent
                {
                    NoteId = note.Id,
                    PlainText = request.Text?.PlainText ?? string.Empty,
                    HtmlContent = request.Text?.HtmlContent ?? string.Empty,
                    RichTextJson = request.Text?.RichTextJson
                };

                break;

            case NoteType.Voice:
                note.VoiceContent = new VoiceNoteContent
                {
                    NoteId = note.Id,
                    TranscriptText = request.Voice?.TranscriptText
                };
                break;

            case NoteType.Image:
                note.ImageContent = new ImageNoteContent
                {
                    NoteId = note.Id,
                    OcrText = request.Image?.OcrText,
                    Caption = request.Image?.Caption
                };
                break;

            case NoteType.HandDraw:
                note.HandDrawContent = new HandDrawNoteContent
                {
                    NoteId = note.Id,
                    DrawingJson = request.HandDraw?.DrawingJson ?? string.Empty,
                    ExtractedText = request.HandDraw?.ExtractedText
                };
                break;
        }
    }

    private static NoteResponse MapToResponse(Note note)
    {

        return new NoteResponse(
            note.Id,
            note.SpaceId,
            note.Title,
            note.Type,
            note.IsPinned,
            note.CreatedAt,
            note.UpdatedAt,
            note.TextContent?.PlainText,
            note.TextContent?.HtmlContent,
            note.TextContent?.RichTextJson,
            note.VoiceContent?.TranscriptText,
            note.ImageContent?.OcrText,
            note.ImageContent?.Caption,
            note.HandDrawContent?.DrawingJson,
            note.HandDrawContent?.ExtractedText,
            note.Assets.Select(a => new NoteAssetItemResponse(
                a.Id,
                a.AssetType,
                a.FileName,
                a.StorageUrl,
                a.MimeType,
                a.SizeInBytes,
                a.DurationSeconds
            ))
            .ToList()
        );
    }

    private async Task<List<NoteAsset>> BuildAssetsAsync( List<AssetPayload>? assets, NoteType noteType, CancellationToken cancellationToken)
    {
        if (assets is null || assets.Count == 0)
            return [];

        var result = new List<NoteAsset>();

        foreach (var asset in assets)
        {
            var saved = await SaveAssetFileAsync(asset, noteType, cancellationToken);

            result.Add(new NoteAsset
            {
                AssetType = asset.AssetType,
                FileName = saved.OriginalFileName,
                StorageUrl = saved.RelativeUrl,
                MimeType = saved.ContentType,
                SizeInBytes = saved.Size,
                DurationSeconds = asset.DurationSeconds
            });
        }

        return result;
    }
}