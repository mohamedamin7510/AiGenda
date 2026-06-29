using AI_genda_API.Abstractions.Const;
using Google.Apis.Auth;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;

namespace AI_genda_API.Services.AuthService;

public class AuthServic(
     UserManager<ExtendedUser> userManager,
     IJWTProvider jWTProvider,
     SignInManager<ExtendedUser> signInManager,
     AppContext context,
     IEmailSender emailSender,
     ILogger<AuthServic> logger,
     IHttpContextAccessor httpContextAccessor,
     IConfiguration configuration
    ) : IAuthService
{

    private readonly SignInManager<ExtendedUser> _SignInManager = signInManager;
    private readonly AppContext _Context = context;
    private readonly IEmailSender _EmailSender = emailSender;
    private readonly ILogger<AuthServic> _Logger = logger;
    private readonly IHttpContextAccessor _HttpContextAccessor = httpContextAccessor;
    private readonly IConfiguration _Configuration = configuration;

    private UserManager<ExtendedUser> _UserManager { get; } = userManager;
    private readonly IJWTProvider _JWTProvider = jWTProvider;
    private readonly int _RefreshTokenExpiryDate = 14;

    public async Task<Result<AuthResponse>?> GetTokenAsync(LoginReq request, CancellationToken? cancellationToken = default)
    {

        var user = await _UserManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Faluire<AuthResponse>(UserErrors.EmailnotFounded);

        var SigninResult = await _SignInManager.PasswordSignInAsync(user, request.Password, false, true);

        if (SigninResult.Succeeded)
        {

            (IEnumerable<string> roles, IEnumerable<string> permissions) = await GetRolesPermission(user);

            var GeneratedTokenInfo = _JWTProvider.GenerateToken(user, roles, permissions);

            var GeneratedrefreshToken = GenerateRefreshToken();

            var ReftokenExpiryDate = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDate);

            user.RefreshTokens.Add(new RefreshToken() { refreshToken = GeneratedrefreshToken, ExpiredAt = ReftokenExpiryDate });

            await _UserManager.UpdateAsync(user);

            return Result.Success(new AuthResponse(user.Id,
                FirstName: user.FirstName!,
                SecondName: user.SecondName!,
                Email: user.Email!,
                Token: GeneratedTokenInfo.Token,
                ExpiredIn: GeneratedTokenInfo.Expiresin * 60,
                RefreshToken: GeneratedrefreshToken,
                ExpiryDate: ReftokenExpiryDate
                ));
        }

        var error = SigninResult.IsLockedOut ? UserErrors.UserLockedOut :
            SigninResult.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.Invalidcredentails;

        return Result.Faluire<AuthResponse>(error);
    }

    public async Task<Result<AuthResponse>?> GetRefreshTokenAsync(ReFTokenReq request, CancellationToken? cancellationToken = null)

    {
        string? userid = _JWTProvider.ValidateToken(request.Token);

        if (userid is null)
            return Result.Faluire<AuthResponse>(UserErrors.InvalidToken);

        var user = await _UserManager.FindByIdAsync(userid);

        if (user is null)
            return Result.Faluire<AuthResponse>(UserErrors.InvalidToken);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Faluire<AuthResponse>(UserErrors.UserLockedOut);

        var reftoken = user.RefreshTokens.FirstOrDefault(x => x.refreshToken == request.RefreshToken && x.IsActive);

        if (reftoken is null)
            return Result.Faluire<AuthResponse>(UserErrors.InvalidToken);

        reftoken.RevokedAt = DateTime.UtcNow;

        await _UserManager.UpdateAsync(user);

        (IEnumerable<string> roles, IEnumerable<string> permissions) = await GetRolesPermission(user);

        var GeneratedTokenInfo = _JWTProvider.GenerateToken(user, roles, permissions);

        var GeneratedrefreshToken = GenerateRefreshToken();

        var ReftokenExpiryDate = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDate);

        user.RefreshTokens.Add(new RefreshToken() { refreshToken = GeneratedrefreshToken, ExpiredAt = ReftokenExpiryDate });

        await _UserManager.UpdateAsync(user);


        return Result.Success(
            new AuthResponse(user.Id,
            FirstName: user.FirstName!,
            SecondName: user.SecondName!,
            Email: user.Email!,
            Token: GeneratedTokenInfo.Token,
            ExpiredIn: GeneratedTokenInfo.Expiresin * 60,
            RefreshToken: GeneratedrefreshToken,
            ExpiryDate: ReftokenExpiryDate
            ));
    }

    public async Task<Result> RevokeRefreshTokenAsync(ReFTokenReq request, CancellationToken? cancellationToken = default)
    {

        string? userid = _JWTProvider.ValidateToken(request.Token);

        if (userid is null)
            return Result.Faluire(UserErrors.InvalidToken);

        var user = await _UserManager.FindByIdAsync(userid);

        if (user is null)
            return Result.Faluire(UserErrors.InvalidToken);

        var reftoken = user.RefreshTokens.FirstOrDefault(x => x.refreshToken == request.RefreshToken && x.IsActive);

        if (reftoken is null) return
                Result.Faluire(UserErrors.InvalidToken);

        reftoken.RevokedAt = DateTime.UtcNow;

        await _UserManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result<RegisterResponse>> ResgisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var IsEmailExisted = await _Context.Users.AnyAsync(x => x.Email == request.Email);

        if (IsEmailExisted)
            return Result.Faluire<RegisterResponse>(UserErrors.EmailDuplicated);

        var NewUser = new ExtendedUser()
        {
            FirstName = request.FirstName,
            SecondName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            PasswordHash = request.Password
        };

        var result = await _UserManager.CreateAsync(NewUser, request.Password);

        if (result.Succeeded)
        {

            var Code = await _UserManager.GenerateEmailConfirmationTokenAsync(NewUser);

            var ReadyToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Code));

            _Logger.LogInformation("Confirmation Code : {code}", ReadyToken);

            await _UserManager.AddToRoleAsync(NewUser, DefaultRoles.Member);

            SendEmailConfirmation(NewUser, ReadyToken);

            var response = NewUser.Adapt<ExtendedUser, RegisterResponse>();

            return Result.Success(response);

        }

        return Result.Faluire<RegisterResponse>
           (new Error("Register.Failed", "The Registeration is not completed ", StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {

        if (await _UserManager.FindByIdAsync(request.userId) is not { } User)
            return Result.Faluire(UserErrors.InvalidCode);


        if (User.EmailConfirmed)
            return Result.Faluire(UserErrors.ActiveConfirmedEmail);

        string Code = request.Code;

        try
        {
            Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
        }
        catch (FormatException)
        {
            return Result.Faluire(UserErrors.InvalidCode);
        }

        var finalresult = await _UserManager.ConfirmEmailAsync(User, Code);

        if (finalresult.Succeeded)
        {
            return Result.Success();
        }
        var error = finalresult.Errors.FirstOrDefault()!;

        return Result.Faluire(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ResendConfirmEmailAsync(ResendConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        if (await _UserManager.FindByEmailAsync(request.email) is not { } User)
            return Result.Success();

        if (User.EmailConfirmed)
            return Result.Faluire(UserErrors.ActiveConfirmedEmail);

        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(await _UserManager.GenerateEmailConfirmationTokenAsync(User)));

        _Logger.LogInformation("Confirmation Code: {code}", code);

        SendEmailConfirmation(User, code);

        return Result.Success();
    }

    public async Task<Result> SendResetPassCodeAsync(ForgetPasswordRequest request, CancellationToken cancellationToken)
    {

        if (await _UserManager.FindByEmailAsync(request.email) is not { } user)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Faluire(UserErrors.EmailNotConfirmed);

        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(await _UserManager.GeneratePasswordResetTokenAsync(user)));

        _Logger.LogInformation(code);

        var origin = _HttpContextAccessor.HttpContext!.Request.Headers.Origin;

        var HtmlBody = EmailBodyBuilder.GenerateEmailBody("ResetPassword", new Dictionary<string, string>()
       {
           {"{{CODE}}",code },
           {"{{FullName}}" , user.FirstName + " " + user.SecondName},
           { "{{URI}}", $"{origin}/Auth/reset-pss/?email={user.Email}&&code={code}"}
       });

        BackgroundJob.Enqueue(() => _EmailSender.SendEmailAsync(user.Email!, "✅ AiGenda Team: Reset Password", HtmlBody));

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
    {

        if (await _UserManager.FindByEmailAsync(request.email) is not { } user)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Faluire(UserErrors.InvalidCode);

        IdentityResult result = null!;
        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.code));

            result = await _UserManager.ResetPasswordAsync(user, code, request.newpassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(_UserManager.ErrorDescriber.InvalidToken());
        }

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.FirstOrDefault();

        return Result.Faluire(new Error(error!.Code, error.Description, StatusCodes.Status401Unauthorized));
    }

    public async Task<Result<AuthResponse>> GoogleLoginAsync(GoogleLoginRequest request, CancellationToken cancellationToken = default)
    {

        var AllowedClientIds = _Configuration.GetSection("Authentication:Google:AllowedClientIds").Get<string[]>();

        GoogleJsonWebSignature.Payload payload = null!;

        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken,

                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = AllowedClientIds
                });
        }
        catch
        {
            return Result.Faluire<AuthResponse>(UserErrors.InvalidToken);
        }


        if (payload.Email is null || !payload.EmailVerified)
            return Result.Faluire<AuthResponse>(UserErrors.Invalidcredentails);


        var user = await _UserManager.FindByLoginAsync(ExternalProviders.GoogleProvider, payload.Subject);

        if (user is null)
        {
            user = await _UserManager.FindByEmailAsync(payload.Email);

            if (user is null)
            {
                user = new ExtendedUser
                {
                    Email = payload.Email,
                    UserName = payload.Email,
                    FirstName = payload.GivenName ?? "Google",
                    SecondName = payload.FamilyName ?? "User",
                    EmailConfirmed = true
                };

                var CreatedUserResult = await _UserManager.CreateAsync(user);

                if (!CreatedUserResult.Succeeded)
                {
                    var error = CreatedUserResult.Errors.First()!;

                    return Result.Faluire<AuthResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
                }

                await _UserManager.AddToRoleAsync(user, DefaultRoles.Member);
            }


            var LoginRecord = new UserLoginInfo(ExternalProviders.GoogleProvider, payload.Subject, ExternalProviders.GoogleProvider);

            var AddLoginResult = await _UserManager.AddLoginAsync(user, LoginRecord);

            if (!AddLoginResult.Succeeded)
            {
                var error = AddLoginResult.Errors.First()!;

                return Result.Faluire<AuthResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }

        }

       if (user.LockoutEnd > DateTime.UtcNow  )
            return Result.Faluire<AuthResponse>(UserErrors.UserLockedOut);

        var response = await LoginWithGoogleExternalApiAsync(user, cancellationToken);

        return Result.Success(response);
    }


    private string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private void SendEmailConfirmation(ExtendedUser User, string code)
    {
        var origin = _HttpContextAccessor.HttpContext?.Request.Headers.Origin;

        var BuilderMessage = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",

        new Dictionary<string, string>
            {
                        {"{{name}}", User.FirstName + " "+ User.SecondName },
                        {"{{code}}", code },
                        { "{{action_url}}", $"{origin}/Auth/confirm-email?userid={User.Id}&&code={code}" }
            }
        );

    
        BackgroundJob.
            Enqueue(() => _EmailSender.SendEmailAsync(User.Email!, "️✅ AiGenda Team: Email Confirmation", BuilderMessage));


        
    }

    private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetRolesPermission(ExtendedUser user)
    {
        var roles = await _UserManager.GetRolesAsync(user);

        var permissions =
            await (from r in _Context.Roles
                   join rp in _Context.RoleClaims on r.Id equals rp.RoleId
                   where roles.Contains(r.Name!)
                   select rp.ClaimValue
                   )
                   .ToListAsync();
        return (roles, permissions);
    }


    private async Task<AuthResponse> LoginWithGoogleExternalApiAsync(ExtendedUser  user, CancellationToken cancellationToken)
    {

        (var userRoles, var permissions) = await GetRolesPermission(user);

        var tokenInfo = _JWTProvider.GenerateToken(user, userRoles, permissions!);

        var refreshToken = GenerateRefreshToken();

        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDate);

        user.RefreshTokens.Add(new RefreshToken
        {
            refreshToken = refreshToken,
            ExpiredAt = refreshTokenExpiry
        });

        await _UserManager.UpdateAsync(user);

        var response = new AuthResponse(
            user.Id,
            user.Email!,
            user.FirstName!,
            user.SecondName!,
            tokenInfo.Token,
            tokenInfo.Expiresin * 60,
            refreshToken,
            refreshTokenExpiry);

        return response;
    }


}
