using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkyBox.DAL.Entities_dbDTOs_;

public class StorageFileEntity
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Название бакета в котором лежит файл
    /// </summary>
    public string BucketName { get; set; } = string.Empty;
    
    /// <summary>
    /// Название файла
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Размер содержимого файла
    /// </summary>
    public long? Length { get; set; }
    
    /// <summary>
    /// Mime тип файла
    /// </summary>
    public string MimeType { get; set; } = string.Empty;
    
    /// <summary>
    /// Расширение файла
    /// </summary>
    public string Extension { get; set; } = string.Empty;
    
    /// <summary>
    /// Путь к файлу в s3 хранилище
    /// </summary>
    public string StoragePath { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата загрузки файла
    /// </summary>
    public DateTimeOffset UploadDate { get; set; }
    
    /// <summary>
    /// Дата последнего доступа к файлу
    /// </summary>
    public DateTimeOffset LastAccessedDate { get; set; }
    
    public Guid UserEntityId { get; set; }
    
    public UserEntity? UserEntity { get; set; }
}

public class FileEntityConfiguration : IEntityTypeConfiguration<StorageFileEntity>
{
    private const int MaxLength = 100;
    
    public void Configure(EntityTypeBuilder<StorageFileEntity> builder)
    {
        builder.ToTable("Files");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.BucketName)
            .IsRequired()
            .HasMaxLength(MaxLength);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(MaxLength);
        
        builder.Property(x => x.Length).IsRequired();
        
        builder.Property(x => x.MimeType)
            .IsRequired()
            .HasMaxLength(MaxLength);
        
        builder.Property(x => x.Extension)
            .IsRequired()
            .HasMaxLength(MaxLength);
        
        builder.Property(x => x.StoragePath)
            .IsRequired()
            .HasMaxLength(MaxLength);
        
        builder.Property(x => x.UploadDate).IsRequired();
        builder.Property(x => x.LastAccessedDate).IsRequired();
        builder.Property(x => x.UserEntityId).IsRequired();
        
        builder.HasOne(x => x.UserEntity)
            .WithMany(x => x.Files)
            .HasForeignKey(x => x.UserEntityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}