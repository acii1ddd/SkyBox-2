using Microsoft.Extensions.DependencyInjection;
using SkyBox.DAL.MappingProfiles;
using SkyBox.DAL.Repositories;
using SkyBox.Domain.Abstractions.Files;

namespace SkyBox.DAL.ConfigurationDI;

public static class ConfigurationExtensions
{
    public static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IFileStorageRepository, FileStorageRepository>();
    }

    public static void RegisterDalProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
            config.AddMaps(typeof(StorageFileProfile).Assembly)
        );
    }
}