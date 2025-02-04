namespace SkyBox.Domain.Abstractions.Cache;

public interface ICacheService
{
    public Task<Guid?> GetByKey(string key);
}