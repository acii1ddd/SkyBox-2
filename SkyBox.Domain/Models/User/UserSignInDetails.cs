namespace SkyBox.Domain.Models.User;

public class UserSignInDetails
{
    public Guid UserId { get; set; }
    
    public UserRole UserRole { get; set; }
    
    public string PasswordHash { get; set; } = string.Empty;
    
}