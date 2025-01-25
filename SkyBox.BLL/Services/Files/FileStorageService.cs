using System.Collections.Immutable;
using Amazon.S3;
using Amazon.S3.Model;
using FluentResults;
using Microsoft.Extensions.Logging;
using SkyBox.Domain.Abstractions.Files;
using SkyBox.Domain.Models;
using SkyBox.Domain.Models.File;

namespace SkyBox.BLL.Services.Files;

public class FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IFileStorageRepository _fileStorageRepository;
    private readonly ILogger<FileStorageService> _logger;
    
    private const string FileWithIdNotFoundPattern = "Файл с Id {0} не найден.";
    private const string UserFilesNotFoundPattern = "{0}: Пользователь с Id {1} не имеет файлов в базе данных.";
    
    private const string BucketName = "user-files";
    private const string TempUserId = "387a29db-2975-4882-a61e-e8e332a74041";

    public FileStorageService(IAmazonS3 s3Client, IFileStorageRepository fileStorageRepository, ILogger<FileStorageService> logger)
    {
        _s3Client = s3Client;
        _fileStorageRepository = fileStorageRepository;
        _logger = logger;
    }

    private static string GetKey(Guid userId, Guid fileId)
    { 
        return $"{userId.ToString()}/{fileId.ToString()}";
    }
    
    public async Task<Result<StorageFile>> UploadFileAsync(Stream fileStream, string fileName, string mimeType)
    {
        try
        {
            // userId брать при авторизации и передавать в этот метод
            var fileId = Guid.NewGuid();
            var key = GetKey(Guid.Parse(TempUserId), fileId);
            
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
                                ?? throw new Exception("Ошибка получения расширения файла. Path is null!"),
                StoragePath = $"{BucketName}/{key}",
                UploadDate = DateTimeOffset.UtcNow,
                LastAccessedDate = DateTimeOffset.MinValue,
                UserId = Guid.Parse(TempUserId) // Брать guid авторизированного типа
            };
            
            _logger.LogInformation("{Source}: Файл {fileName} загружен успешно.", 
                nameof(FileStorageService), fileId);
            
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
            _logger.LogInformation("{Source}: Файл {fileId} получен успешно.", 
                nameof(FileStorageService), fileId);
            
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

    public async Task<Result<StorageFile>> DeleteFileAsync(Guid fileId)
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
            
            _ = await _s3Client.DeleteObjectAsync(
                BucketName, 
                GetKey(Guid.Parse(TempUserId), fileId)
            );
            
            var deletedFile = await _fileStorageRepository.DeleteAsync(fileId);
            
            _logger.LogInformation("{Source}: Файл {fileId} удален успешно.", 
                nameof(FileStorageService), fileId);
            
            // deletedFile не может быть null, так как
            // уже проверили существование объекта в GetByIdAsync
            return deletedFile!;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Source}: Ошибка во время получения файла по Id.", 
                nameof(FileStorageService));
            
            return Result.Fail(new Error(e.Message));
        }
    }

    public async Task<Result<ImmutableList<StorageFile>>> GetByUserIdAsync(Guid userId)
    {
        try
        {
            var userFiles = await _fileStorageRepository.GetByUserIdAsync(userId);
            var files = userFiles.ToImmutableList();

            if (!files.IsEmpty)
            {
                return files;
            }
            
            _logger.LogError("{Source}: Пользователь с Id {userId} не имеет файлов в базе данных.", 
                nameof(FileStorageService), userId);

            return Result.Fail(new Error(string.Format(UserFilesNotFoundPattern, 
                nameof(FileStorageService), userId.ToString())));
        }        
        catch (Exception e)
        {
            _logger.LogError(e, "{Source}: Ошибка во время получения файлов пользователя" +
                " c Id {userId}.", userId, nameof(FileStorageService));
            
            return Result.Fail(new Error(e.Message));
        }
    }
}