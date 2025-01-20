using Microsoft.Extensions.DependencyInjection;
using SkyBox.BLL.Services;
using SkyBox.Domain.Abstractions.Files;

namespace SkyBox.BLL.ConfigurationDI;

public static class ConfigurationExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IFileStorageService, FileStorageService>();
    }
}