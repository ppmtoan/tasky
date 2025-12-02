using ModuleTest.SaasService.Application;
using Volo.Abp.Modularity;

namespace ModuleTest.SaasService;

[DependsOn(
    typeof(SaasServiceApplicationModule),
    typeof(SaasServiceDomainTestModule)
    )]
public class SaasServiceApplicationTestModule : AbpModule
{

}
