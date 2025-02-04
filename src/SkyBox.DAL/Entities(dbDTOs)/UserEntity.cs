using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkyBox.Domain.Models.User;

namespace SkyBox.DAL.Entities_dbDTOs_;

public class UserEntity
{
    public Guid Id { get; set; }
    
    public UserRole Role { get; set; }
    
    public string UserName { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public List<StorageFileEntity> Files { get; set; } = [];
}

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    private const int MaxLength = 100;
    
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Role)
            .IsRequired()
            .HasDefaultValue(UserRole.Default);
        
        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(MaxLength);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(MaxLength);
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(MaxLength);

        builder.HasMany(x => x.Files)
            .WithOne(x => x.UserEntity)
            .HasForeignKey(x => x.UserEntityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}