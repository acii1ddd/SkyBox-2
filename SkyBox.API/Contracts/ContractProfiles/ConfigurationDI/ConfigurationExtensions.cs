namespace SkyBox.API.Contracts.ContractProfiles.ConfigurationDI;

public static class ConfigurationExtensions
{
    public static void RegisterContractProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            // берем сборку, в которой находится StorageFileContractProfile
            config.AddMaps(typeof(StorageFileContractProfile).Assembly);
        });
    }
}