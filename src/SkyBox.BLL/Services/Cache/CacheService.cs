using SkyBox.Domain.Abstractions.Cache;

namespace SkyBox.BLL.Services.Cache;

public class CacheService : ICacheService
{
    // Реализовать с Redis
    public async Task<Guid?> GetByKey(string key)
    {
        return Guid.Parse(new string("123"));
    }
}