using ModuleTest.SaasService.Application;
using ModuleTest.SaasService.Localization;
using Volo.Abp.Application.Services;

namespace ModuleTest.SaasService;

public abstract class SaasServiceAppService : ApplicationService
{
    protected SaasServiceAppService()
    {
        LocalizationResource = typeof(SaasServiceResource);
        ObjectMapperContext = typeof(SaasServiceApplicationModule);
    }
}
