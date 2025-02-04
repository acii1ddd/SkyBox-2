namespace SkyBox.Domain.Abstractions.Users;

public interface IPasswordHashService
{
    public string HashPassword(string password);
    
    public bool Verify(string password, string passwordHash);
}