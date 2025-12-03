using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using ModuleTest.SaasService.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Saas.Host;
using Volo.Saas.Tenant;

namespace ModuleTest.SaasService;

[DependsOn(
    typeof(SaasServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(SaasHostHttpApiModule),
    typeof(SaasTenantHttpApiModule)
)]
public class SaasServiceHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(SaasServiceHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
            .Get<SaasServiceResource>()
            .AddBaseTypes(typeof(AbpUiResource));
        });

        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(SaasServiceHttpApiModule).Assembly, opts =>
            {
                opts.RootPath = "saas-service";
            });
        });
    }
}
