using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleTest.SaasService.TenantAdmin;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace ModuleTest.SaasService.Controllers.TenantAdmin;

[RemoteService(Name = SaasServiceRemoteServiceConsts.RemoteServiceName)]
[Area(SaasServiceRemoteServiceConsts.ModuleName)]
[Route("api/saas/tenant-admin")]
[AllowAnonymous] // For testing purposes - remove in production
public class TenantAdminController : AbpControllerBase, ITenantAdminAppService
{
    private readonly ITenantAdminAppService _tenantAdminAppService;

    public TenantAdminController(ITenantAdminAppService tenantAdminAppService)
    {
        _tenantAdminAppService = tenantAdminAppService;
    }

    [HttpGet]
    [Route("dashboard")]
    public virtual Task<TenantDashboardDto> GetDashboardAsync()
    {
        return _tenantAdminAppService.GetDashboardAsync();
    }
}
