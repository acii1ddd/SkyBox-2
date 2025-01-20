using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SkyBox.DAL.Context;
using SkyBox.DAL.Entities_dbDTOs_;
using SkyBox.Domain.Abstractions.Files;
using SkyBox.Domain.Models;

namespace SkyBox.DAL.Repositories;

public class FileStorageRepository : IFileStorageRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    
    public FileStorageRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Добавить файл в базу данных
    /// </summary>
    /// <param name="storageFile"></param>
    /// <returns>Добавленный файл</returns>
    public async Task<StorageFile> AddAsync(StorageFile storageFile)
    {
        var file = _mapper.Map<StorageFileEntity>(storageFile);
        await _dbContext.StorageFiles.AddAsync(file);
        await _dbContext.SaveChangesAsync();
        // mapping back 
        return _mapper.Map<StorageFile>(file);
    }
    
    /// <summary>
    /// Получить все файлы
    /// </summary>
    /// <returns>Список файлов(может быть пустым)</returns>
    public async Task<IEnumerable<StorageFile>> GetAllAsync()
    {
        return _mapper.Map<IEnumerable<StorageFile>>(
            await _dbContext.StorageFiles
                .AsNoTracking()
                .ToListAsync()
        );
    }

    
    /// <summary>
    /// Получение всех файлов пользователя 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Список файлов либо пустой список</returns>
    public async Task<IEnumerable<StorageFile>> GetByUserIdAsync(Guid userId)
    {
        return _mapper.Map<IEnumerable<StorageFile>>(
            await _dbContext.StorageFiles
                .AsNoTracking()
                .Where(file => file.UserEntityId == userId)
                .ToListAsync()
        );
    }

    /// <summary>
    /// Получение файла по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Файл по-заданному id либо null, если не было такого файла</returns>
    public async Task<StorageFile?> GetByIdAsync(Guid id)
    {
        return _mapper.Map<StorageFile>(
            await _dbContext.StorageFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id)
        );
    }

    /// <summary>
    /// Удаление файла
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Удаленный файл либо null, если не было такого файла</returns>
    public async Task<StorageFile?> DeleteAsync(Guid id)
    {
        var file = _dbContext.StorageFiles
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);

        if (file is not null)
        {
            await _dbContext.StorageFiles
                .Where(f => f.Id == id)
                .ExecuteDeleteAsync();
        }

        return _mapper.Map<StorageFile>(file);
    }
}