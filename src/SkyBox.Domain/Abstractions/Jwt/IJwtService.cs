using FluentResults;
using SkyBox.Domain.Models.Auth;

namespace SkyBox.Domain.Abstractions.Jwt;

public interface IJwtService
{
    public Task<Result<AuthTokenModel>> GetNewTokensPair(string refreshToken);
}