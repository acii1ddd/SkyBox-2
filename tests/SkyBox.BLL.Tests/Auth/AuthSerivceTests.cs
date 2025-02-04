using Microsoft.Extensions.Options;
using Moq;
using SkyBox.BLL.Services.Auth;
using SkyBox.Domain.Abstractions.Users;
using SkyBox.Domain.Models.User;
using SkyBox.FileStorageConfiguration.Auth;

namespace SkyBox.BLL.Tests.Auth;

public class AuthSerivceTests
{
    private readonly AuthService _authService;
    
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHashService> _passwordHashServiceMock;

    public AuthSerivceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHashServiceMock = new Mock<IPasswordHashService>();
        var authSettings = new AuthSettings
        {
            Internal = new InternalAuthSettings
            {
                Issuer = "SkyBox.API",
                Audience = "SkyBox.UI",
                Secret = "secretsecretsecretsecretsecretsecretsecretsecretsecretsecretsecretsecretsecretsecret",
                LifeTime = 30
            }
        };
        
        _authService = new AuthService(
            _userRepositoryMock.Object, 
            _passwordHashServiceMock.Object,
            Options.Create(authSettings)
        );
    }

    [Fact]
    public async Task SignIn_UserNotFound_ShouldReturnFailResult()
    {
        // Arrange
        var signInModel = new SignInModel
        {
            UserName = "test-user",
            Password = "123"
        };
        
        _userRepositoryMock
            .Setup(x => x.GetDetailsByUserNameAsync(signInModel.UserName))
            .ReturnsAsync((UserSignInDetails?) null);
        
        // Act
        var authTokenModelResult = await _authService.SignIn(signInModel);
            
        // Assert
        Assert.True(authTokenModelResult.IsFailed);
        Assert.Single(authTokenModelResult.Errors);
        Assert.Equal($"Пользователь с именем {signInModel.UserName} не найден.",
            authTokenModelResult.Errors.First().Message
        );
    }
    
    [Fact]
    public async Task SignIn_InvalidPassword_ShouldReturnFailResult()
    {
        // Arrange
        var signInModel = new SignInModel
        {
            UserName = "test-user",
            Password = "123"
        };

        var userSignInDetails = new UserSignInDetails
        {
            UserId = Guid.NewGuid(),
            PasswordHash = "TestPassword",
            UserRole = UserRole.Default
        };
        
        _userRepositoryMock
            .Setup(x => x.GetDetailsByUserNameAsync(signInModel.UserName))
            .ReturnsAsync(userSignInDetails);

        _passwordHashServiceMock
            .Setup(x => x.Verify(signInModel.Password, userSignInDetails.PasswordHash))
            .Returns(false);
        
        // Act
        var authTokenModelResult = await _authService.SignIn(signInModel);
            
        // Assert
        Assert.True(authTokenModelResult.IsFailed);
        Assert.Single(authTokenModelResult.Errors);
        Assert.Equal("Неверный пароль.", 
            authTokenModelResult.Errors.First().Message
        );
    }
    
    [Fact]
    public async Task SignIn_ValidCredentials_ShouldReturnSuccessResult()
    {
        // Arrange
        var signInModel = new SignInModel
        {
            UserName = "test-user",
            Password = "123"
        };

        var userSignInDetails = new UserSignInDetails
        {
            UserId = Guid.NewGuid(),
            UserRole = UserRole.Default,
            PasswordHash = "TestPassword",
        };
        
        _userRepositoryMock
            .Setup(x => x.GetDetailsByUserNameAsync(signInModel.UserName))
            .ReturnsAsync(userSignInDetails);

        _passwordHashServiceMock
            .Setup(x => x.Verify(signInModel.Password, userSignInDetails.PasswordHash))
            .Returns(true);
        
        // Act
        var authTokenModelResult = await _authService.SignIn(signInModel);
            
        // Assert
        Assert.True(authTokenModelResult.IsSuccess);
        Assert.Empty(authTokenModelResult.Errors);
        Assert.NotNull(authTokenModelResult.Value);
        
        Assert.Equal(authTokenModelResult.Value.AuthAccessTokenModel.UserId, userSignInDetails.UserId);
        Assert.Equal(authTokenModelResult.Value.AuthAccessTokenModel.UserRole, userSignInDetails.UserRole);
    }
}