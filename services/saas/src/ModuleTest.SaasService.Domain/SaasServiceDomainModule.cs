using Volo.Abp.Modularity;
using Volo.Saas;

namespace ModuleTest.SaasService;

[DependsOn(
    typeof(SaasServiceDomainSharedModule),
    typeof(SaasDomainModule)
)]
public class SaasServiceDomainModule : AbpModule
{
}
