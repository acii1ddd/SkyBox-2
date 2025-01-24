using SkyBox.Domain.Abstractions.Users;

namespace SkyBox.BLL.Services.Users;

public class PasswordHashService : IPasswordHashService
{
    public string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string passwordHash) 
        => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}