using Microsoft.Extensions.DependencyInjection;
using SkyBox.DAL.MappingProfiles;
using SkyBox.DAL.Repositories;
using SkyBox.Domain.Abstractions.Files;
using SkyBox.Domain.Abstractions.Users;

namespace SkyBox.DAL.ConfigurationDI;

public static class ConfigurationExtensions
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IFileStorageRepository, FileStorageRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }

    public static IServiceCollection RegisterDalProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
            config.AddMaps(typeof(StorageFileProfile).Assembly)
        );

        return services;
    }
}