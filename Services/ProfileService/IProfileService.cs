using AI_genda_API.Contracts.Profile;
using AI_genda_API.Contracts.ProfileSetting;
using BucketSurvey.Api.Contract.User;

namespace AI_genda_API.Services.ProfileService;

public interface IProfileService
{
    Task<Result<ProfileResponse>> GetAsync(string Id);
    Task<Result<ProfileResponse>> UpdateAsync(string Id, ProfileRequest request, CancellationToken cancellationToken);
    Task<Result> ChangePasswordAsync(string Id, ChangePasswordRequest request);
    Task<Result> ChangeEmailAsync(string UserId, changeEmailRequest request);
    Task<Result> ConfirmChangeEmailAsync(ConfirmChangeEmailRequest request);
    Task<Result<AvatarResponse>> UploadAvatarAsync(string Id, UploadAvatarRequest request, CancellationToken cancellationToken);
    Task<Result<AvatarResponse>> GetAvatarAsync(string Id);
    Task<Result> RemoveAvatarAsync(string Id, CancellationToken cancellationToken);
}
