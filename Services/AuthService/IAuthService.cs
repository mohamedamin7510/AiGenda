namespace AI_genda_API.Services.AuthService;


public interface IAuthService
{
   public  Task<Result<AuthResponse>?> GetTokenAsync(LoginReq request, CancellationToken? cancellationToken = default);

   public  Task<Result<AuthResponse>?> GetRefreshTokenAsync(ReFTokenReq request, CancellationToken? cancellationToken = default);

   public Task<Result> RevokeRefreshTokenAsync(ReFTokenReq request, CancellationToken? cancellationToken = default);
   
   public Task<Result<RegisterResponse>> ResgisterAsync(RegisterRequest request, CancellationToken cancellationToken);
  
   public Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken);
  
   public Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request, CancellationToken cancellationToken);

   public Task<Result> SendResetPassCodeAsync(ForgetPasswordRequest request, CancellationToken cancellationToken);
 
   public Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken);

   public Task<Result<AuthResponse>> GoogleLoginAsync(GoogleLoginRequest request, CancellationToken cancellationToken = default);


}
