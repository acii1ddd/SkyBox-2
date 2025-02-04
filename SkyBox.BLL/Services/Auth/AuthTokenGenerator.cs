using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SkyBox.Domain.Models.Auth;
using SkyBox.FileStorageConfiguration.Auth;

namespace SkyBox.BLL.Services.Auth;

public static class AuthTokenGenerator
{
    public static AuthAccessTokenModel GenerateAccessToken(GenerateTokenPayload payload, InternalAuthSettings authSettings)
    {
        var expires = DateTimeOffset.UtcNow.AddMinutes(authSettings.LifeTime);
        
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
        {
            new("userId", payload.UserId.ToString()),
            new(ClaimTypes.Role, payload.UserRole.ToString())
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires.UtcDateTime,
            Issuer = authSettings.Issuer,
            Audience = authSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Secret)),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var securityToken = tokenHandler.CreateToken(descriptor);
        
        var token = tokenHandler.WriteToken(securityToken);

        return new AuthAccessTokenModel
        {
            UserId = payload.UserId,
            AccessToken = token,
            Expires = expires.UtcDateTime,
            UserRole = payload.UserRole,
        };
    }

    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber); // fill array with bytes
            return Convert.ToBase64String(randomNumber);
        }
    }
}
