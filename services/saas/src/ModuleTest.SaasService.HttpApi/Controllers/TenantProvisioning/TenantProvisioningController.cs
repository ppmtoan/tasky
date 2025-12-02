using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleTest.SaasService.TenantProvisioning;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace ModuleTest.SaasService.Controllers.TenantProvisioning;

/// <summary>
/// Handles complete tenant provisioning including tenant creation, admin setup, and subscription activation
/// </summary>
[RemoteService(Name = SaasServiceRemoteServiceConsts.RemoteServiceName)]
[Area(SaasServiceRemoteServiceConsts.ModuleName)]
[Route("api/saas/tenant-provisioning")]
public class TenantProvisioningController : AbpControllerBase, ITenantProvisioningAppService
{
    private readonly ITenantProvisioningAppService _tenantProvisioningAppService;

    public TenantProvisioningController(ITenantProvisioningAppService tenantProvisioningAppService)
    {
        _tenantProvisioningAppService = tenantProvisioningAppService;
    }

    /// <summary>
    /// Provisions a new tenant with subscription and administrator account in a single operation
    /// </summary>
    /// <param name="input">Tenant provisioning details including tenant name, admin credentials, and subscription plan</param>
    /// <returns>Provisioning result with tenant ID, subscription ID, and access information</returns>
    /// <response code="200">Tenant provisioned successfully</response>
    /// <response code="400">Invalid input or tenant name already exists</response>
    [HttpPost]
    [Route("provision")]
    [AllowAnonymous] // Can be made public for self-service signup
    public virtual Task<TenantProvisioningResultDto> ProvisionTenantAsync(TenantProvisioningRequestDto input)
    {
        return _tenantProvisioningAppService.ProvisionTenantAsync(input);
    }
}
