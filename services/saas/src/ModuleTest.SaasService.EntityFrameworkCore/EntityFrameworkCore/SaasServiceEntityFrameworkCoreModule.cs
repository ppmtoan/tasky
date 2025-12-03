using Microsoft.Extensions.DependencyInjection;
using ModuleTest.SaasService.EntityFrameworkCore.Repositories;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Saas.EntityFrameworkCore;

namespace ModuleTest.SaasService.EntityFrameworkCore;

[DependsOn(
    typeof(SaasServiceDomainModule),
    typeof(SaasEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule)
)]
public class SaasServiceEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        SaasServiceEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<SaasServiceDbContext>(options =>
        {
            options.ReplaceDbContext<ITenantManagementDbContext>();

                /* includeAllEntities: true allows to use IRepository<TEntity, TKey> also for non aggregate root entities */
            options.AddDefaultRepositories(includeAllEntities: true);
            
            // Register custom repositories
            options.AddRepository<ModuleTest.SaasService.Aggregates.EditionAggregate.Edition, EditionRepository>();
            options.AddRepository<ModuleTest.SaasService.Aggregates.SubscriptionAggregate.Subscription, SubscriptionRepository>();
            options.AddRepository<ModuleTest.SaasService.Aggregates.BillingAggregate.Invoice, InvoiceRepository>();
        });
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.Databases.Configure(
                SaasServiceDbProperties.ConnectionStringName,
                db =>
                {
                    db.MappedConnections.Add("AbpTenantManagement");
                }
            );
        });

        Configure<AbpDbContextOptions>(options =>
        {            
            options.Configure<SaasServiceDbContext>(c =>
            {
                c.UseNpgsql(b =>
                {
                    b.MigrationsHistoryTable("__SaasService_Migrations");
                });
            });
        });
    }
}
