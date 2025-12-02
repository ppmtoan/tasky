using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleTest.SaasService.TenantProvisioning;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace ModuleTest.SaasService.Controllers.TenantProvisioning;

[RemoteService(Name = SaasServiceRemoteServiceConsts.RemoteServiceName)]
[Area(SaasServiceRemoteServiceConsts.ModuleName)]
[Route("api/saas-service/tenant-provisioning")]
public class TenantProvisioningController : AbpControllerBase, ITenantProvisioningAppService
{
    private readonly ITenantProvisioningAppService _tenantProvisioningAppService;

    public TenantProvisioningController(ITenantProvisioningAppService tenantProvisioningAppService)
    {
        _tenantProvisioningAppService = tenantProvisioningAppService;
    }

    [HttpPost]
    [Route("provision")]
    [AllowAnonymous] // Can be made public for self-service signup
    public virtual Task<TenantProvisioningResultDto> ProvisionTenantAsync(TenantProvisioningRequestDto input)
    {
        return _tenantProvisioningAppService.ProvisionTenantAsync(input);
    }
}
