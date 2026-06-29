using AI_genda_API.Contracts.Profile;
using AI_genda_API.Contracts.ProfileSetting;
using AI_genda_API.Services.ProfileService;
using BucketSurvey.Api.Contract.User;
using Microsoft.Extensions.Caching.Memory;

namespace AI_genda_API.Controllers;

[Route("me")]
[ApiController]
[Authorize]
public class ProfilesController(IProfileService profileSettingService , IMemoryCache memoryCache) : ControllerBase
{
    private readonly IProfileService _ProfileService = profileSettingService;


    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _ProfileService.GetAsync(User.GetUserId()!);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await _ProfileService.UpdateAsync(User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _ProfileService.ChangePasswordAsync(User.GetUserId()!, request);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPost("change-email")]
    public async Task<IActionResult> ConfirmChangeEmail([FromBody] changeEmailRequest request)
    {
        var result = await _ProfileService.ChangeEmailAsync(User.GetUserId()!, request);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("confirm-change-email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ConfirmChangeEmailRequest request)
    {
        var result = await _ProfileService.ConfirmChangeEmailAsync(request);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarRequest request, CancellationToken cancellationToken)
    {
        var result = await _ProfileService.UploadAvatarAsync(User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("avatar")]
    public async Task<IActionResult> GetAvatar()
    {
        var result = await _ProfileService.GetAvatarAsync(User.GetUserId()!);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("avatar")]
    public async Task<IActionResult> DeleteAvatar(CancellationToken cancellationToken)
    {
        var result = await _ProfileService.RemoveAvatarAsync(User.GetUserId()!, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }



}
