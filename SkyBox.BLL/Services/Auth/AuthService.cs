using FluentResults;
using Microsoft.Extensions.Options;
using SkyBox.Domain.Abstractions.Auth;
using SkyBox.Domain.Abstractions.Users;
using SkyBox.Domain.Models.Auth;
using SkyBox.Domain.Models.User;
using SkyBox.FileStorageConfiguration.Auth;

namespace SkyBox.BLL.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly AuthSettings _authSettings;

    public AuthService(
        IUserRepository userRepository, 
        IPasswordHashService passwordHashService,
        IOptions<AuthSettings> authOptions)
    {
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _authSettings = authOptions?.Value
                        ?? throw new Exception("Секция с настройками аутентификации в конфигурационном файле некорректна.");
    }

    public async Task<Result<AuthTokenModel>> SignIn(SignInModel signInModel)
    {
        // валидация какае-то
        
        var userSignInDetails = await _userRepository.GetByUserNameAsync(signInModel.UserName);
        
        if (userSignInDetails is null)
        {
            return Result.Fail(new Error($"Пользователь с именем {signInModel.UserName} не найден."));
        }
        
        // есть такой пользователь с таким userName в базе данных
        var verifyResult = _passwordHashService.Verify(signInModel.Password, userSignInDetails.PasswordHash);
        if (verifyResult is false)
        {
            return Result.Fail(new Error("Неверный пароль."));
        }

        var token = AuthTokenGenerator.GenerateToken(
            new GenerateTokenPayload
        {   
            UserId = userSignInDetails.UserId,
            UserRole = userSignInDetails.UserRole,
        }, _authSettings.Internal);
        
        return token;
    }
}
