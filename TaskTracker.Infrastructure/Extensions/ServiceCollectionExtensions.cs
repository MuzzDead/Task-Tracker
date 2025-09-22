using Azure.Storage.Blobs;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskTracker.Application.Archice;
using TaskTracker.Application.Common.Interfaces.Auth;
using TaskTracker.Application.Common.Interfaces.Services;
using TaskTracker.Application.OpenAi;
using TaskTracker.Application.Payment;
using TaskTracker.Application.Storage;
using TaskTracker.Domain.Options;
using TaskTracker.Infrastructure.Auth;
using TaskTracker.Infrastructure.BackgroundServices;
using TaskTracker.Infrastructure.Services;

namespace TaskTracker.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(options
           => configuration.GetSection("JwtOptions").Bind(options));

        var jwtOptions = configuration
            .GetSection("JwtOptions")
            .Get<JwtOptions>()!;

        var secretKey = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey)
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("X-Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        var hangfireConn = configuration.GetConnectionString("HangfireConnection");

        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(hangfireConn));

        services.AddScoped<IArchiveBoardsJob, ArchiveBoardsJob>();

        services.AddHangfireServer();


        services.AddSingleton<IServiceBusService>(provider =>
            new ServiceBusService(configuration["ServiceBus:ConnectionString"]));

        services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddMemoryCache();
        services.Configure<AzureOpenAIOptions>(configuration.GetSection(AzureOpenAIOptions.SectionName));
        services.AddSingleton<IChatSessionStore, MemoryChatSessionStore>();
        services.AddScoped<IChatService, AzureOpenAIChatService>();

        services.AddScoped<IPaymentService, StripePaymentService>();

        services.AddHostedService<CleanupExpiredTokensService>();

        services.AddHttpContextAccessor();

        return services;
    }

    public static IServiceCollection AddFunctionServices(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.Configure<CosmosDbOptions>(configuration.GetSection("CosmosDb"));
        services.AddSingleton<CosmosClient>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
            return new CosmosClient(opts.ConnectionString);
        });

        services.AddScoped<ICosmosDbService, CosmosDbService>();

        services.Configure<BlobStorageOptions>(configuration.GetSection("AzureBlobStorage"));
        services.AddScoped<IBlobService, BlobService>();

        return services;
    }
}
