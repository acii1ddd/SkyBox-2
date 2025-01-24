using SkyBox.Domain.Models;
using SkyBox.Domain.Models.File;

namespace SkyBox.Domain.Abstractions.Files;

public interface IFileStorageRepository
{
    public Task<StorageFile> AddAsync(StorageFile storageFile);
    
    public Task<IEnumerable<StorageFile>> GetAllAsync();

    public Task<IEnumerable<StorageFile>> GetByUserIdAsync(Guid userId);
    
    public Task<StorageFile?> GetByIdAsync(Guid id);

    public Task<StorageFile?> DeleteAsync(Guid id);
}