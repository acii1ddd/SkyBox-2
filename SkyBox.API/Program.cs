using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SkyBox.API.Contracts;
using SkyBox.API.Contracts.ContractProfiles.ConfigurationDI;
using SkyBox.API.Middlewares;
using SkyBox.BLL.ConfigurationDI;
using SkyBox.DAL;
using SkyBox.DAL.ConfigurationDI;
using SkyBox.DAL.Context;
using SkyBox.FileStorageConfiguration;
using SkyBox.FileStorageConfiguration.Auth;

namespace SkyBox.API;

public class Program
{
    private const string CorsPolicyName = "CorsPolicy";
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        // CUSTOM
        
        // context
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("PostgresDbConnection")
                ?? throw new ApplicationException("Не удалось определить строку подключения для регистрации контекста базы данных.");

            options.UseNpgsql(connectionString, optionsBuilder =>
            {
                // Связанные коллекции будут загружены одним запросом к базе данных (один SQL-запрос с JOIN)
                optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
            });
        });
        
        // S3 (minio)
        // парсим секцию S3Options из appsettings.json в класс AWSOptions
        builder.Services.Configure<S3Options>(builder.Configuration.GetSection("S3Options"));
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var awsOptions = sp.GetRequiredService<IOptions<S3Options>>().Value;

            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsOptions.Region),
                ServiceURL = awsOptions.ServiceUrl,
                ForcePathStyle = awsOptions.ForcePathStyle,
                UseHttp = awsOptions.UseHttp
            };
            
            var credentials = new BasicAWSCredentials(awsOptions.AccessKey, awsOptions.SecretKey);
            return new AmazonS3Client(credentials, s3Config);
        });
        
        // builder.Services.AddCors(options =>
        // {
        //     options.AddPolicy(CorsPolicyName, corsPolicyBuilder => 
        //         corsPolicyBuilder
        //             .AllowAnyOrigin()
        //             .AllowAnyMethod()
        //             .AllowAnyHeader());
        // });
        
        // Settings
        builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Auth"));
        
        //DI
        builder.Services.RegisterRepositories();
        builder.Services.RegisterServices();
        builder.Services.RegisterDalProfiles();
        builder.Services.RegisterContractProfiles();
        
        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            DbInitializer.Initialize(context);
        }
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.UseAuthentication();

        app.MapControllers();
        
        // custom middlewares
        app.UseMiddleware<JwtAuthMiddleware>();
        
        app.Run();
    }
}