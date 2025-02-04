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
        
        const string testPasswordHash = "$2a$11$B8iXtl2QbDxzLWRLp0A89OGgccLrEtwl/LK4UWJAcYLolBq90K7BC";
    
        //context.Database.Migrate();
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "user",
            PasswordHash = testPasswordHash,
            Email = "user@email.com"
        };
        
        context.Users.Add(user);
        context.SaveChanges();
    }
}
