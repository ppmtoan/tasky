using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using ModuleTest.AdministrationService.EntityFrameworkCore;
using ModuleTest.SaasService.EntityFrameworkCore;
using ModuleTest.Shared.Hosting.AspNetCore;
using StackExchange.Redis;
using System.Collections.Generic;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.Authentication.JwtBearer.DynamicClaims;
using Volo.Abp.AspNetCore.MultiTenancy;
// using Volo.Abp.BackgroundJobs.RabbitMQ;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EventBus.Kafka;
using Volo.Abp.Modularity;
// using Volo.Abp.MongoDB;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace ModuleTest.Shared.Hosting.Microservices;

[DependsOn(
    // typeof(AbpMongoDbModule), // Un-comment if you are using mongodb in any microservice
    typeof(ModuleTestSharedHostingAspNetCoreModule),
    // typeof(AbpBackgroundJobsRabbitMqModule),
    typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpEventBusKafkaModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(SaasServiceEntityFrameworkCoreModule),
    typeof(AdministrationServiceEntityFrameworkCoreModule)
)]
public class ModuleTestSharedHostingMicroservicesModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<WebRemoteDynamicClaimsPrincipalContributorOptions>(options =>
        {
            options.IsEnabled = true;
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = true;
        });

        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "ModuleTest:";
        });

        var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);

        context.Services
            .AddDataProtection()
            .SetApplicationName("ModuleTest")
            .PersistKeysToStackExchangeRedis(redis, "ModuleTest-Protection-Keys");

        context.Services.AddSingleton<IDistributedLockProvider>(_ =>
            new RedisDistributedSynchronizationProvider(redis.GetDatabase()));

        if (hostingEnvironment.IsDevelopment())
        {
            IdentityModelEventSource.ShowPII = true;
        }

        context.Services.AddHttpClient();
    }
}

public static class MicroserviceExtensions
{
    public static ServiceConfigurationContext ConfigureMicroservice(
        this ServiceConfigurationContext context,
        string name
    )
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        context.ConfigureAuthentication(configuration, name);
        context.ConfigureCache(name);
        context.ConfigureSwaggerServices(configuration, name);

        return context;
    }

    public static ServiceConfigurationContext ConfigureAuthentication(
        this ServiceConfigurationContext context,
        IConfiguration configuration,
        string audience
    )
    {
        context
            .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddAbpJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>(
                    "AuthServer:RequireHttpsMetadata"
                );
                options.Audience = audience;
            });

        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });

        return context;
    }

    public static ServiceConfigurationContext ConfigureCache(
        this ServiceConfigurationContext context,
        string keyPrefix
    )
    {
        context.Services.Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = $"{keyPrefix}:";
        });

        return context;
    }

    public static ServiceConfigurationContext ConfigureSwaggerServices(
        this ServiceConfigurationContext context,
        IConfiguration configuration,
        string name,
        string version = "v1"
    )
    {
        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"]!,
            new Dictionary<string, string> { { name, $"{name} API" } },
            options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo { Title = $"{name} API", Version = version }
                );
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );

        return context;
    }
}