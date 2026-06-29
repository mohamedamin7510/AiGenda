using AI_genda_API.Contracts.Profile;
using AI_genda_API.Contracts.ProfileSetting;
using BucketSurvey.Api.Contract.User;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;

namespace AI_genda_API.Services.ProfileService;

public class ProfileService( UserManager<ExtendedUser> userManager, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor,
    ILogger<ProfileService> logger,
    IMemoryCache memoryCache,
    IWebHostEnvironment webHostEnvironment):IProfileService
  {
    private readonly UserManager<ExtendedUser> _UserManager = userManager;
    private readonly IEmailSender _EmailSender = emailSender;
    private readonly IHttpContextAccessor _HttpContextAccessor = httpContextAccessor;
    private readonly ILogger<ProfileService> _Logger = logger;
    private readonly IWebHostEnvironment _WebHostEnvironment = webHostEnvironment;
    private readonly IMemoryCache _MemoryCache = memoryCache;
    private readonly string _PerfixCache = "Profile";


    public async Task<Result<ProfileResponse>> GetAsync(string Id)
    {

        string key = _PerfixCache + "_" + Id;

        var user = await _MemoryCache.GetOrCreate(key, async cachedData =>
        {
            cachedData.SlidingExpiration = TimeSpan.FromHours(1);

            return await _UserManager.FindByIdAsync(Id);
        }
        )!;

        if (user is null || user.IsDisabled)
            return Result.Faluire<ProfileResponse>(UserErrors.UserNotFounded);

        var response = user.Adapt<ExtendedUser, ProfileResponse>();

        return Result.Success(response);
    }

    public async Task<Result<ProfileResponse>> UpdateAsync(string Id, ProfileRequest request, CancellationToken cancellationToken)
    {
        var user = await _UserManager.FindByIdAsync(Id);

        if (user is null || user.IsDisabled)
            return Result.Faluire<ProfileResponse>(UserErrors.UserIsDeleted);

        user.FirstName =   request.FirstName;
        user.SecondName =  request.SecondName;
        user.DateOfBirth = request.DateOfBirth;
        user.JobTitle = request.JobTitle;

        _MemoryCache.Remove(_PerfixCache + "_" + Id);

        await _UserManager.UpdateAsync(user);

        var response = user.Adapt<ProfileResponse>();

        return Result.Success(response);
    }

    public async Task<Result> ChangePasswordAsync(string Id, ChangePasswordRequest request)
    {
        var user = await _UserManager.FindByIdAsync(Id);

        var result = await _UserManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Faluire(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ChangeEmailAsync(string UserId, changeEmailRequest request)
    {
        var user = await _UserManager.FindByIdAsync(UserId);

        if (user is null)
            return Result.Faluire(UserErrors.UserNotFounded);

        var token = await _UserManager.GenerateChangeEmailTokenAsync(user, request.newemail);

        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        _Logger.LogInformation("token:{Token}", code);

        var origin = _HttpContextAccessor.HttpContext!.Request.Headers.Origin;

        var htmlMessage = EmailBodyBuilder.GenerateEmailBody("ChangeEmail", new Dictionary<string, string>
        {
            { "{{name}}", user.FirstName + " " + user.SecondName },
            { "{{newemail}}", request.newemail },
            { "{{code}}", code },
            { "{{URI}}",$"{origin}/Auth/change-email?userid={user.Id}&&code={code}&&newemail={request.newemail}"}
        });

        BackgroundJob.Enqueue(() => _EmailSender.SendEmailAsync(request.newemail, "✅ AiGenda Team: New Email Confirmation", htmlMessage!));

        return Result.Success();
    }

    public async Task<Result> ConfirmChangeEmailAsync(ConfirmChangeEmailRequest request)
    {
        if (await _UserManager.FindByIdAsync(request.Id) is not { } User)
            return Result.Faluire(UserErrors.InvalidCode);

        if (User.Email == request.newemail)
            return Result.Faluire(UserErrors.ActiveConfirmedEmail);

        var IsEmailExist = await _UserManager.Users.AnyAsync(x => x.Email == request.newemail);

        if (IsEmailExist)
            return Result.Faluire(UserErrors.EmailDuplicated);

        string Code = request.Code;

        try
        {
            Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
        }
        catch (FormatException)
        {
            return Result.Faluire(UserErrors.InvalidCode);
        }

        var finalresult = await _UserManager.ChangeEmailAsync(User, request.newemail, Code);

        if (finalresult.Succeeded)
        {
            User.UserName = request.newemail;
            User.NormalizedUserName = request.newemail.ToUpper();
            await _UserManager.UpdateAsync(User);
            _MemoryCache.Remove(_PerfixCache + "_" + User.Id);

            return Result.Success();
        }

        var error = finalresult.Errors.FirstOrDefault()!;
        return Result.Faluire(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result<AvatarResponse>> UploadAvatarAsync(string Id, UploadAvatarRequest request, CancellationToken cancellationToken)
    {
        var user = await _UserManager.FindByIdAsync(Id);

        if (user is null || user.IsDisabled)
            return Result.Faluire<AvatarResponse>(UserErrors.UserNotFounded);

        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

        var avatarsFolder = Path.Combine(_WebHostEnvironment.WebRootPath ?? Path.Combine(_WebHostEnvironment.ContentRootPath, "wwwroot"), "uploads", "avatars");
        
        Directory.CreateDirectory(avatarsFolder);

        DeleteExistingAvatarFileIfLocal(user.AvatarUrl, avatarsFolder);

        var fileName = $"{user.Id}_{Guid.NewGuid():N}{extension}";

        var physicalPath = Path.Combine(avatarsFolder, fileName);

        await using (var stream = new FileStream(physicalPath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        var relativeUrl = $"/uploads/avatars/{fileName}";

        user.AvatarUrl = relativeUrl;

        _MemoryCache.Remove(_PerfixCache + "_" + Id);

        await _UserManager.UpdateAsync(user);

        return Result.Success(new AvatarResponse(user.AvatarUrl));
    }

    public async Task<Result<AvatarResponse>> GetAvatarAsync(string Id)
    {
        var user = await _UserManager.FindByIdAsync(Id);

        if (user is null || user.IsDisabled)
            return Result.Faluire<AvatarResponse>(UserErrors.UserNotFounded);

        return Result.Success(new AvatarResponse(user.AvatarUrl));
    }

    public async Task<Result> RemoveAvatarAsync(string Id, CancellationToken cancellationToken)
    {
        var user = await _UserManager.FindByIdAsync(Id);

        if (user is null || user.IsDisabled)
            return Result.Faluire(UserErrors.UserNotFounded);

        var avatarsFolder = Path.Combine(_WebHostEnvironment.WebRootPath ?? Path.Combine(_WebHostEnvironment.ContentRootPath, "wwwroot"), "uploads", "avatars");
       
        DeleteExistingAvatarFileIfLocal(user.AvatarUrl, avatarsFolder);

        user.AvatarUrl = null;

        _MemoryCache.Remove(_PerfixCache + "_" + Id);

        await _UserManager.UpdateAsync(user);

        return Result.Success();
    }

    private static void DeleteExistingAvatarFileIfLocal(string? avatarUrl, string avatarsFolder)
    {
        if (string.IsNullOrWhiteSpace(avatarUrl))
            return;

        if (!avatarUrl.StartsWith("/uploads/avatars/", StringComparison.OrdinalIgnoreCase))
            return;

        var fileName = Path.GetFileName(avatarUrl);

        var filePath = Path.Combine(avatarsFolder, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
