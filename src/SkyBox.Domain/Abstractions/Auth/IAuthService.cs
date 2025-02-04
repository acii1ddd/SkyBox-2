using FluentResults;
using SkyBox.Domain.Models.Auth;
using SkyBox.Domain.Models.User;

namespace SkyBox.Domain.Abstractions.Auth;

public interface IAuthService
{
    public Task<Result<AuthTokenModel>> SignIn(SignInModel signInModel);
}