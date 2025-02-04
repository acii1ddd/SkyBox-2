using FluentResults;
using Microsoft.Extensions.Options;
using SkyBox.BLL.Services.Auth;
using SkyBox.Domain.Abstractions.Cache;
using SkyBox.Domain.Abstractions.Jwt;
using SkyBox.Domain.Abstractions.Users;
using SkyBox.Domain.Models.Auth;
using SkyBox.FileStorageConfiguration.Auth;

namespace SkyBox.BLL.Services.JWT;

public class JwtService : IJwtService
{
    private readonly ICacheService _cacheService;
    private readonly IUserRepository _userRepository;
    private readonly AuthSettings _authSettings;

    public JwtService(ICacheService cacheService, IUserRepository userRepository, IOptions<AuthSettings> authOptions)
    {
        _cacheService = cacheService;
        _userRepository = userRepository;
        _authSettings = authOptions?.Value
                        ?? throw new ArgumentNullException(nameof(authOptions), 
                            "Секция с настройками аутентификации в конфигурационном файле некорректна.");
    }
    
    public async Task<Result<AuthTokenModel>> GetNewTokensPair(string refreshToken)
    {
        var userId = await _cacheService.GetByKey(refreshToken);
        if (userId is null)
        {
            return Result.Fail(new Error($"В кэше нет записи с ключом {refreshToken}"));
        }
        
        // так как не null, извлекаем Guid из Guid?
        var userJwtDetails = await _userRepository.GetDetailsByUserIdAsync(userId.Value);
        if (userJwtDetails is null)
        {
            return Result.Fail(new Error($"Пользователь с Id {userId.ToString()} не найден."));
        }
        
        // генерируем новую пару ключей
        var newAuthAccessTokenModel = AuthTokenGenerator.GenerateAccessToken(new GenerateTokenPayload
        {
            UserId = userJwtDetails.UserId,
            UserRole = userJwtDetails.UserRole,
        }, _authSettings.Internal);

        var newRefreshToken = AuthTokenGenerator.GenerateRefreshToken();
        
        return new AuthTokenModel
        {
            AuthAccessTokenModel = newAuthAccessTokenModel,
            RefreshToken = newRefreshToken
        };
    }
}