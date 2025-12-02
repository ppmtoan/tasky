using ModuleTest.AdministrationService;
using ModuleTest.AdministrationService.EntityFrameworkCore;
using ModuleTest.IdentityService;
using ModuleTest.IdentityService.EntityFrameworkCore;
using ModuleTest.ProductService;
using ModuleTest.ProductService.EntityFrameworkCore;
using ModuleTest.SaasService;
using ModuleTest.SaasService.EntityFrameworkCore;
using ModuleTest.Shared.Hosting;
using Volo.Abp.Modularity;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;

namespace ModuleTest.DbMigrator;

[DependsOn(
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(ModuleTestSharedHostingModule),
    typeof(IdentityServiceEntityFrameworkCoreModule),
    typeof(IdentityServiceApplicationContractsModule),
    typeof(SaasServiceEntityFrameworkCoreModule),
    typeof(SaasServiceApplicationContractsModule),
    typeof(AdministrationServiceEntityFrameworkCoreModule),
    typeof(AdministrationServiceApplicationContractsModule),
    typeof(ProductServiceApplicationContractsModule),
    typeof(ProductServiceEntityFrameworkCoreModule)
)]
public class ModuleTestDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "ModuleTest:"; });
    }
}
