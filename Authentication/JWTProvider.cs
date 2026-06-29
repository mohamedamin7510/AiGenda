
using AI_genda_API.Abstractions.Const;
using System.Text;
using System.Text.Json;

namespace AI_genda_API.Authentication;

public class JWTProvider(IOptions<JWTOptions> JWToptions) : IJWTProvider
{
     private JWTOptions _JWToptions { get; } = JWToptions.Value;

    public (string Token , int Expiresin ) GenerateToken(ExtendedUser extendedUser , IEnumerable<string> roles , IEnumerable<string> permissions)
    {
        Claim[] claims = new Claim[] {
                new Claim(JwtRegisteredClaimNames.Sub, extendedUser.Id),
                new Claim(JwtRegisteredClaimNames.Email, extendedUser.Email!),
                new Claim(JwtRegisteredClaimNames.GivenName, extendedUser.FirstName!),
                new Claim(JwtRegisteredClaimNames.FamilyName, extendedUser.SecondName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) ,
                new Claim(nameof(roles) , JsonSerializer.Serialize(roles) , JsonClaimValueTypes.JsonArray),
                new Claim(nameof(permissions) , JsonSerializer.Serialize(permissions) , JsonClaimValueTypes.JsonArray)
        };

         var SymmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWToptions.SymmetricKey));

         var SigningCredentials = new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _JWToptions.Issuer,
            audience: _JWToptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_JWToptions.ExpirtMiniuites),
            signingCredentials: SigningCredentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),_JWToptions.ExpirtMiniuites);
    }

    public string? ValidateToken(string token )
    {

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        try
        {

          jwtSecurityTokenHandler.ValidateToken(
             token
             ,
             new TokenValidationParameters()
             {
                 ValidateIssuerSigningKey = true,
                 ValidateIssuer = true,
                 ValidateAudience = true,
                 ValidateLifetime = true,

                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWToptions!.SymmetricKey)),
                 ValidIssuer = _JWToptions.Issuer,
                 ValidAudience = _JWToptions.Audience,
                 ClockSkew = TimeSpan.Zero
             }
             ,
                out SecurityToken securityToken
             );


            var jwtSecurityToken = (JwtSecurityToken)securityToken;
            return jwtSecurityToken.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)!.Value;
        }
        catch 
        {
            return null; 
        }



    
    }
}
