using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace ModuleTest.SaasService.TenantProvisioning;

/// <summary>
/// Tenant provisioning service
/// Note: Apply [AllowAnonymous] or [Authorize] at controller/method level
/// </summary>
public interface ITenantProvisioningAppService : IApplicationService
{
    Task<TenantProvisioningResultDto> ProvisionTenantAsync(TenantProvisioningRequestDto input);
}
