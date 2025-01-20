using FluentResults;
using SkyBox.Domain.Models;

namespace SkyBox.Domain.Abstractions.Files;

public interface IFileStorageService
{
    public Task<Result<StorageFile>> UploadFileAsync(Stream fileStream, string fileName, string mimeType);

    public Task<Result<(Stream fileStream, StorageFile storageFile)>> GetByIdAsync(Guid fileId);
}