using System.Globalization;
using System.Text;
using FluentResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SkyBox.BLL.Services.Auth;
using SkyBox.Domain.Models.Auth;
using SkyBox.Domain.Models.User;
using SkyBox.FileStorageConfiguration.Auth;
using Xunit.Abstractions;

namespace SkyBox.BLL.Tests.Auth;

public class JwtTokenTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly AuthSettings _authSettings;
    
    public JwtTokenTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _authSettings = new AuthSettings
        {
            Internal = new InternalAuthSettings
            {
                Issuer = "SkyBox.API",
                Audience = "SkyBox.UI",
                Secret = "secretsecretsecretsecretsecretsecretsecretsecretsecretsecretsecretsecretsecretsecret",
                LifeTime = 1
            }
        };
    }

    [Fact]
    public void GenerateAccessTokenTest()
    {
        var userId = Guid.NewGuid();
            
        var authTokenModel = AuthTokenGenerator.GenerateAccessToken(new GenerateTokenPayload
        {
            UserId = userId,
            UserRole = UserRole.Default
        }, _authSettings.Internal);
        
        Assert.NotNull(authTokenModel);
        Assert.Equal(authTokenModel.UserId, userId);
        Assert.Equal(UserRole.Default, authTokenModel.UserRole);
        Assert.NotNull(authTokenModel.AccessToken);
        Assert.Equal(authTokenModel.Expires.ToString("hh:mm"),
            DateTimeOffset.UtcNow.AddMinutes(_authSettings.Internal.LifeTime).UtcDateTime.ToString("hh:mm"));
        
        _testOutputHelper.WriteLine(authTokenModel.UserId.ToString());
        _testOutputHelper.WriteLine(authTokenModel.AccessToken);
        _testOutputHelper.WriteLine(authTokenModel.Expires.ToString(CultureInfo.CurrentCulture));
        _testOutputHelper.WriteLine(authTokenModel.UserRole.ToString());
    }

    [Fact]
    public async Task ValidateTokenTest_ShouldReturnValidResult()
    {
        var userId = Guid.NewGuid();

        var authTokenModel = AuthTokenGenerator.GenerateAccessToken(new GenerateTokenPayload
        {
            UserId = userId,
            UserRole = UserRole.Default
        }, _authSettings.Internal);

        var tokenHandler = new JsonWebTokenHandler();

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.Internal.Secret)),
            ValidIssuer = _authSettings.Internal.Issuer,
            ValidAudience = _authSettings.Internal.Audience
        };
        
        var validationResult = await tokenHandler.ValidateTokenAsync(
            authTokenModel.AccessToken, 
            parameters
        );
        
        Assert.True(validationResult.IsValid);
        _testOutputHelper.WriteLine(validationResult.SecurityToken.ToString());
    }
    
    [Fact]
    public async Task ValidateTokenTest_ShouldReturnInvalidResult()
    {
        var userId = Guid.NewGuid();

        var authTokenModel = AuthTokenGenerator.GenerateAccessToken(new GenerateTokenPayload
        {
            UserId = userId,
            UserRole = UserRole.Default
        }, _authSettings.Internal);

        // update token to invalid
        var authTokenArray = authTokenModel.AccessToken.ToCharArray();
        authTokenArray[0] = '1';
        var newToken = authTokenArray.ToString();
        authTokenModel.AccessToken = newToken!;
        
        var tokenHandler = new JsonWebTokenHandler();

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.Internal.Secret)),
            ValidIssuer = _authSettings.Internal.Issuer,
            ValidAudience = _authSettings.Internal.Audience
        };
        
        var validationResult = await tokenHandler.ValidateTokenAsync(
            authTokenModel.AccessToken, 
            parameters
        );
        
        Assert.False(validationResult.IsValid);
    }
    
    [Fact]
    public void GenerateRefreshTokenTest()
    {
        var refreshToken = AuthTokenGenerator.GenerateRefreshToken();
        
        // Assert
        Assert.NotNull(refreshToken);
        _testOutputHelper.WriteLine(refreshToken);
    }
}