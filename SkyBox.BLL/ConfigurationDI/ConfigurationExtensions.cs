using Microsoft.Extensions.DependencyInjection;
using SkyBox.BLL.Services;
using SkyBox.BLL.Services.Auth;
using SkyBox.BLL.Services.Files;
using SkyBox.BLL.Services.Users;
using SkyBox.Domain.Abstractions.Auth;
using SkyBox.Domain.Abstractions.Files;
using SkyBox.Domain.Abstractions.Users;

namespace SkyBox.BLL.ConfigurationDI;

public static class ConfigurationExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        return services;
    }
}