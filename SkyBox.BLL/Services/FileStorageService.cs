using Amazon.S3;
using Amazon.S3.Model;
using FluentResults;
using Microsoft.Extensions.Logging;
using SkyBox.Domain.Abstractions.Files;
using SkyBox.Domain.Models;

namespace SkyBox.BLL.Services;

public class FileStorageService : IFileStorageService
{
    private const string FileWithIdNotFoundPattern = "Файл с Id {0} не найден.";
    
    private readonly IAmazonS3 _s3Client;
    private readonly IFileStorageRepository _fileStorageRepository;
    private readonly ILogger<FileStorageService> _logger;

    private const string BucketName = "user-files";
    
    public FileStorageService(IAmazonS3 s3Client, IFileStorageRepository fileStorageRepository, ILogger<FileStorageService> logger)
    {
        _s3Client = s3Client;
        _fileStorageRepository = fileStorageRepository;
        _logger = logger;
    }

    public async Task<Result<StorageFile>> UploadFileAsync(Stream fileStream, string fileName, string mimeType)
    {
        try
        {
            const string tempUserId = "dd67edab-1135-42e7-9047-331e33875f37";
            // userId брать при авторизации и передавать в этот метод
            var userId = Guid.Parse(tempUserId);
            var fileId = Guid.NewGuid();
            var key = $"{userId.ToString()}/{fileId.ToString()}";
            
            //Размер необходимо зафиксировать до того, как будет вычитан поток
            //который освободится после прочтения
            var streamLength = fileStream.Length;
            
            // add to storage
            await _s3Client.PutObjectAsync(
                new PutObjectRequest {
                    Key = key,
                    BucketName = BucketName,
                    InputStream = fileStream,
                    ContentType = mimeType
                }
            );
            
            // add to db
            var storageFile = new StorageFile
            {
                Id = fileId,
                BucketName = BucketName,
                Name = fileName,
                Length = streamLength,
                MimeType = mimeType,
                Extension = Path.GetExtension(fileName)
                                ?? throw new Exception("Error with getting extension. Path is null!"),
                StoragePath = $"{BucketName}/{key}",
                UploadDate = DateTimeOffset.UtcNow,
                LastAccessedDate = DateTimeOffset.MinValue,
                UserId = userId // Брать guid авторизированного типа
            };
            
            _logger.LogInformation($"File {fileName} uploaded successfully.");
            return await _fileStorageRepository.AddAsync(storageFile);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Source}: Ошибка во время загрузки файла в S3 хранилище.", 
                nameof(FileStorageService));
            
            return Result.Fail(new Error(e.Message));
        }
    }

    public async Task<Result<(Stream fileStream, StorageFile storageFile)>> GetByIdAsync(Guid fileId)
    {
        try
        {
            var fileInfo = await _fileStorageRepository.GetByIdAsync(fileId);
            
            if (fileInfo is null)
            {
                _logger.LogError("{Source}: Файл с Id {FileId} не найден в базе данных",
                    nameof(FileStorageService),
                    fileId);
                
                return Result.Fail(new Error(string.Format(FileWithIdNotFoundPattern, fileId)));
            }
            
            using var file = await _s3Client.GetObjectAsync
            (
                    fileInfo.BucketName, 
                    $"{fileInfo.UserId.ToString()}/{fileId.ToString()}"
            );
            
            // скопировали поток responseStream в другой поток, так как 
            // при освобождении ресурсов file (using) поток будет закрыт
            await using var responseStream = file.ResponseStream;
            var ms = new MemoryStream();
            await responseStream.CopyToAsync(ms);
            
            if (ms.CanSeek)
            {
                ms.Seek(0, SeekOrigin.Begin);
            }
            // возвращаем окрытый поток ms
            return (ms, fileInfo);
            // responseStream.DisposeAsync()
            // file.Dispose()
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Source}: Ошибка во время получения файла по Id.", 
                nameof(FileStorageService));
            
            return Result.Fail(new Error(e.Message));
        }
    }


    // получить файлы для определенного юзера
    // получить ссылку на файл для скачивания (можно по Id файла по сути)
    // удалить файл по Id
}