using Microsoft.EntityFrameworkCore;
using SkyBox.DAL.Context;
using SkyBox.DAL.Entities_dbDTOs_;

namespace SkyBox.DAL;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }
    
        //context.Database.Migrate();
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "user",
            PasswordHash = "123",
            Email = "user@email.com"
        };
        
        context.Users.Add(user);
        context.SaveChanges();
    }
}
