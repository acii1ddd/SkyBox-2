using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkyBox.DAL.Entities_dbDTOs_;

public class UserEntity
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public List<StorageFileEntity> Files { get; set; } = [];
}

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    private const int MAX_LENGTH = 100;
    
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(MAX_LENGTH);

        builder.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(MAX_LENGTH);
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(MAX_LENGTH);

        builder.HasMany(x => x.Files)
            .WithOne(x => x.UserEntity)
            .HasForeignKey(x => x.UserEntityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}