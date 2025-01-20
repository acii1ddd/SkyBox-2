namespace SkyBox.Domain.Models;

public class StorageFile
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
    
    /// <summary>
    /// Идентификатор владельца файла
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Владелец файла
    /// </summary>
    public User? User { get; set; }
}
