using Microsoft.AspNetCore.RateLimiting;

namespace AI_genda_API.Controllers;



[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _AuthService = authService;

    [HttpPost("")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginReq loginReq, CancellationToken cancellationToken = default)
    {
        var Response = await _AuthService.GetTokenAsync(loginReq ,  cancellationToken);

        return Response!.IsSuccess ? Ok(Response.Value): Response.ToProblem();
    }


    [HttpPut("refresh")]
    public async Task<IActionResult> GetRefreshTokenAsync([FromBody] ReFTokenReq Tokens ,CancellationToken cancellationToken)
    {
       var response = await  _AuthService.GetRefreshTokenAsync(Tokens,cancellationToken=default);


        return response is null ? BadRequest("InvalidToken!") : Ok(response.Value);
    }
   

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] ReFTokenReq Tokens, CancellationToken cancellationToken)
    {
        var RevokingResult = await _AuthService.RevokeRefreshTokenAsync(Tokens, cancellationToken = default);

        return RevokingResult.IsSuccess ? Ok("Revoked Successefuly!") : RevokingResult.ToProblem();
    }
    

    [HttpPost("register")]
    public async Task<IActionResult> Register ([FromBody] RegisterRequest request ,  CancellationToken cancellationToken)
    {
        var result = await _AuthService.ResgisterAsync(request , cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        var resutl = await _AuthService.ConfirmEmailAsync(request, cancellationToken);

        return resutl.IsSuccess ? Ok()
            : resutl.ToProblem();
    }


    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        var resutl = await _AuthService.ResendConfirmEmailAsync(request, cancellationToken);

        return resutl.IsSuccess ? Ok()
            : resutl.ToProblem();
    }


   [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var resutl = await _AuthService.SendResetPassCodeAsync(request, cancellationToken);

        return resutl.IsSuccess ? Created() : resutl.ToProblem();
    }


    [HttpPut("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var resutl = await _AuthService.ResetPasswordAsync(request, cancellationToken);

        return resutl.IsSuccess ? NoContent() : resutl.ToProblem();
    }



    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _AuthService.GoogleLoginAsync(request , cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }



}
