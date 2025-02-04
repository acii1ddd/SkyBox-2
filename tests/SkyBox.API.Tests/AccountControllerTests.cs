using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SkyBox.API.Contracts.Auth;
using SkyBox.API.Controllers;
using SkyBox.Domain.Abstractions.Auth;
using SkyBox.Domain.Models.Auth;
using SkyBox.Domain.Models.User;

namespace SkyBox.API.Tests;

public class AccountControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AccountController _accountController;
    private readonly DefaultHttpContext _httpContext;

    public AccountControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _mapperMock = new Mock<IMapper>();
        _httpContext = new DefaultHttpContext();
        
        _accountController = new AccountController(_authServiceMock.Object, _mapperMock.Object);

        _accountController.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContext
        };
    }

    [Fact]
    public async Task SignIn_ShouldReturn_BadRequest()
    {
        // Arrange
        var signInRequest = new SignInRequest
        {
            UserName = "user",
            Password = "123"
        };
        
        var signInModel = new SignInModel
        {
            UserName = "user",
            Password = "123"
        };
        
        _mapperMock.Setup(x => x.Map<SignInModel>(signInRequest))
            .Returns(signInModel);
        

        _authServiceMock.Setup(x => x.SignIn(signInModel))
            .ReturnsAsync(Result.Fail(new Error("Auth error")));
        
        // Act
        var controllerResult = await _accountController.SignIn(signInRequest);
        
        // Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(controllerResult);
        Assert.NotNull(badRequestObjectResult.Value);
    }
    
    [Fact]
    public async Task SignIn_ShouldReturn_200_WithAuthT()
    {
        // Arrange
        var signInRequest = new SignInRequest
        {
            UserName = "user",
            Password = "123"
        };
        
        var signInModel = new SignInModel
        {
            UserName = "user",
            Password = "123"
        };
        
        _mapperMock.Setup(x => x.Map<SignInModel>(signInRequest))
            .Returns(signInModel);
        
        var authAccessTokenModel = new AuthAccessTokenModel
        {
            UserId = Guid.NewGuid(),
            UserRole = UserRole.Admin,
            AccessToken = "test_access_token",
            Expires = DateTimeOffset.UtcNow.AddDays(7).UtcDateTime  
        };
    
        var authTokenModel = new AuthTokenModel
        {
            AuthAccessTokenModel = authAccessTokenModel,
            RefreshToken = "test_refresh_token"
        };
        
        _authServiceMock.Setup(x => x.SignIn(signInModel))
            .ReturnsAsync(Result.Ok(authTokenModel));
        
        _mapperMock.Setup(x => x.Map<SignInResponse>(authAccessTokenModel))
            .Returns(new SignInResponse
        {
            UserId = authAccessTokenModel.UserId,
            UserRole = authAccessTokenModel.UserRole,
            AccessToken= authAccessTokenModel.AccessToken,
            Expires = authAccessTokenModel.Expires,
        });
        
        // Act
        var controllerResult = await _accountController.SignIn(signInRequest);
        
        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(controllerResult);
        Assert.NotNull(okObjectResult.Value);
        var response = Assert.IsType<SignInResponse>(okObjectResult.Value);
        
        Assert.Equal(authAccessTokenModel.UserId, response.UserId);
        Assert.Equal(authAccessTokenModel.UserRole, response.UserRole);
        Assert.Equal(authAccessTokenModel.AccessToken, response.AccessToken);
        
        var setCookieHeader = _httpContext.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains("RefreshToken", setCookieHeader);
        Assert.Contains("HttpOnly", setCookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Secure", setCookieHeader, StringComparison.OrdinalIgnoreCase);
    }
}
