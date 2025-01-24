using SkyBox.Domain.Models.User;

namespace SkyBox.Domain.Abstractions.Users;

public interface IUserRepository
{
    public Task<UserSignInDetails?> GetByUserNameAsync(string userName);
}