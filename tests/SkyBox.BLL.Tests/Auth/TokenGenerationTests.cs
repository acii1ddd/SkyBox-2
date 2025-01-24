using System.Globalization;
using SkyBox.BLL.Services.Auth;
using SkyBox.Domain.Models.Auth;
using SkyBox.Domain.Models.User;
using SkyBox.FileStorageConfiguration.Auth;
using Xunit.Abstractions;

namespace SkyBox.BLL.Tests.Auth;

public class TokenGenerationTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly AuthSettings _authSettings;
    
    public TokenGenerationTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _authSettings = new AuthSettings
        {
            Internal = new InternalAuthSettings
            {
                Issuer = "SkyBox.API",
                Audience = "SkyBox.UI",
                Secret = "secretsecretsecretsecretsecretsecretsecretsecretsecretsecretsecretsecretsecretsecret",
                LifeTime = 30
            }
        };
    }

    [Fact]
    public void GenerateTokenTest()
    {
        var userId = Guid.NewGuid();
            
        var authTokenModel = AuthTokenGenerator.GenerateToken(new GenerateTokenPayload
        {
            UserId = userId,
            UserRole = UserRole.Default
        }, _authSettings.Internal);
        
        Assert.NotNull(authTokenModel);
        Assert.Equal(authTokenModel.UserId, userId);
        Assert.Equal(UserRole.Default, authTokenModel.UserRole);
        Assert.NotNull(authTokenModel.Token);
        Assert.Equal(authTokenModel.Expires.ToString("hh:mm"),
            DateTimeOffset.UtcNow.AddMinutes(_authSettings.Internal.LifeTime).UtcDateTime.ToString("hh:mm"));
        
        _testOutputHelper.WriteLine(authTokenModel.UserId.ToString());
        _testOutputHelper.WriteLine(authTokenModel.Token);
        _testOutputHelper.WriteLine(authTokenModel.Expires.ToString(CultureInfo.CurrentCulture));
        _testOutputHelper.WriteLine(authTokenModel.UserRole.ToString());
    }
}