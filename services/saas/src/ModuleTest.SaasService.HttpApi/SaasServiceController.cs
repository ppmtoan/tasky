using ModuleTest.SaasService.Localization;
using Volo.Abp.AspNetCore.Mvc;
namespace ModuleTest.SaasService;
public abstract class SaasServiceController : AbpControllerBase
{
    protected SaasServiceController()
    {
        LocalizationResource = typeof(SaasServiceResource);
    }
}