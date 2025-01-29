using System.Security.Claims;
using System.Text;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SkyBox.API.Contracts.ContractProfiles.ConfigurationDI;
using SkyBox.API.Extensions;
using SkyBox.BLL.ConfigurationDI;
using SkyBox.DAL;
using SkyBox.DAL.ConfigurationDI;
using SkyBox.DAL.Context;
using SkyBox.Domain.Models.User;
using SkyBox.FileStorageConfiguration;
using SkyBox.FileStorageConfiguration.Auth;
using ILogger = Serilog.ILogger;

namespace SkyBox.API;

public class Program
{
    //private const string CorsPolicyName = "CorsPolicy";
    
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
        var authSection = builder.Configuration.GetSection("Auth");
        builder.Services.Configure<AuthSettings>(authSection);
        
        // DI
        builder.Services
            .RegisterRepositories()
            .RegisterServices()
            .RegisterDalProfiles()
            .RegisterContractProfiles()
            .AddSwagger(); // jwt token in header

        // Authentication
        var authSettings = authSection.Get<AuthSettings>();
        if (authSettings is null)
        {
            throw new InvalidOperationException("Missing authentication settings. Check appsettings.json Auth section.");
        }
        
        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Internal.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = authSettings.Internal.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authSettings.Internal.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // убираем доп погрешность в 5 минут
                };
            });

        builder.Services.AddAuthorization(authOptions =>
        {
            authOptions.AddPolicy(
                nameof(UserRole.Default),
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(ClaimTypes.Role, nameof(UserRole.Default));
                }
            );
        });
        
        // Serilog logging
        builder.Host.UseSerilog((context, _, configuration) =>
        {
            configuration.Enrich.FromLogContext();
            configuration.ReadFrom.Configuration(context.Configuration);
        });
        
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
        
        // custom middlewares
        //app.UseMiddleware<JwtAuthMiddleware>();
        
        app.UseHttpsRedirection();
        
        // first
        app.UseAuthentication();
        // second
        app.UseAuthorization();
        
        app.MapControllers();
        
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        try
        {
            app.Run();
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "{Source}: Unhandled exception", e.Source);
        }
    }
}