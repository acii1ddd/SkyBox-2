using SkyBox.API.Contracts.Users;

namespace SkyBox.API.Contracts.StorageFiles;

public record GetStorageFileResponse
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Название бакета в котором лежит файл
    /// </summary>
    public string BucketName { get; init; } = string.Empty;
    
    /// <summary>
    /// Название файла
    /// </summary>
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// Размер содержимого файла
    /// </summary>
    public long? Length { get; init; }
    
    /// <summary>
    /// Mime тип файла
    /// </summary>
    public string MimeType { get; init; } = string.Empty;
    
    /// <summary>
    /// Расширение файла
    /// </summary>
    public string Extension { get; init; } = string.Empty;
    
    /// <summary>
    /// Путь к файлу в s3 хранилище
    /// </summary>
    public string StoragePath { get; init; } = string.Empty;
    
    /// <summary>
    /// Дата загрузки файла
    /// </summary>
    public DateTimeOffset UploadDate { get; init; }
    
    /// <summary>
    /// Дата последнего доступа к файлу
    /// </summary>
    public DateTimeOffset LastAccessedDate { get; init; }
    
    /// <summary>
    /// Идентификатор владельца файла
    /// </summary>
    public Guid UserId { get; init; }
    
    /// <summary>
    /// Владелец файла
    /// </summary>
    public GetUserResponse? User { get; init; }
}