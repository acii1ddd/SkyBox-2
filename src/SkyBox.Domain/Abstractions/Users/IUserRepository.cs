using SkyBox.Domain.Models.User;

namespace SkyBox.Domain.Abstractions.Users;

public interface IUserRepository
{
    public Task<UserSignInDetails?> GetDetailsByUserNameAsync(string userName);

    public Task<UserJwtDetails?> GetDetailsByUserIdAsync(Guid userId);
}