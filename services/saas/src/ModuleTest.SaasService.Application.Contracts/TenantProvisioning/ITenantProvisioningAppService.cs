using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ModuleTest.SaasService.TenantProvisioning;

public interface ITenantProvisioningAppService : IApplicationService
{
    Task<TenantProvisioningResultDto> ProvisionTenantAsync(TenantProvisioningRequestDto input);
}
