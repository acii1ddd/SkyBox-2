using System.Collections.Immutable;
using FluentResults;
using SkyBox.Domain.Models;
using SkyBox.Domain.Models.File;

namespace SkyBox.Domain.Abstractions.Files;

public interface IFileStorageService
{
    public Task<Result<StorageFile>> UploadFileAsync(Stream fileStream, string fileName, string mimeType);

    public Task<Result<(Stream fileStream, StorageFile storageFile)>> GetByIdAsync(Guid fileId);

    public Task<Result<ImmutableList<StorageFile>>> GetByUserIdAsync(Guid userId);

    public Task<Result<StorageFile>> DeleteFileAsync(Guid fileId);
}