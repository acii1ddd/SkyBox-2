namespace SkyBox.Domain.Models.User;

public class UserJwtDetails
{
    public Guid UserId { get; set; }
    
    public UserRole UserRole { get; set; }
}