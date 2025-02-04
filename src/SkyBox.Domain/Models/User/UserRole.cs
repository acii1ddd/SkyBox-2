namespace SkyBox.Domain.Models.User;

public enum UserRole
{
    /// <summary>
    /// Обычный пользователь
    /// </summary>
    Default = 0,
    
    /// <summary>
    /// Администратор
    /// </summary>
    Admin = 1,
}