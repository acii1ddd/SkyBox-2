namespace SkyBox.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public List<StorageFile> Files { get; set; } = [];
}