using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleTest.SaasService.HostAdmin;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace ModuleTest.SaasService.Controllers.HostAdmin;

[RemoteService(Name = SaasServiceRemoteServiceConsts.RemoteServiceName)]
[Area(SaasServiceRemoteServiceConsts.ModuleName)]
[Route("api/saas-service/host-admin")] // Changed to avoid conflict with ABP's built-in SaaS module
[AllowAnonymous] // For testing purposes - remove in production
public class HostAdminController : AbpControllerBase, IHostAdminAppService
{
    private readonly IHostAdminAppService _hostAdminAppService;

    public HostAdminController(IHostAdminAppService hostAdminAppService)
    {
        _hostAdminAppService = hostAdminAppService;
    }

    [HttpGet]
    [Route("metrics")]
    public virtual Task<HostMetricsDto> GetMetricsAsync()
    {
        return _hostAdminAppService.GetMetricsAsync();
    }

    [HttpGet]
    [Route("tenants")]
    public virtual Task<PagedResultDto<TenantDetailDto>> GetTenantsAsync(PagedAndSortedResultRequestDto input)
    {
        return _hostAdminAppService.GetTenantsAsync(input);
    }

    [HttpGet]
    [Route("tenants/{tenantId}")]
    public virtual Task<TenantDetailDto> GetTenantDetailAsync(Guid tenantId)
    {
        return _hostAdminAppService.GetTenantDetailAsync(tenantId);
    }

    [HttpPost]
    [Route("tenants/{tenantId}/activate")]
    public virtual Task ActivateTenantAsync(Guid tenantId)
    {
        return _hostAdminAppService.ActivateTenantAsync(tenantId);
    }

    [HttpPost]
    [Route("tenants/{tenantId}/deactivate")]
    public virtual Task DeactivateTenantAsync(Guid tenantId)
    {
        return _hostAdminAppService.DeactivateTenantAsync(tenantId);
    }
}
