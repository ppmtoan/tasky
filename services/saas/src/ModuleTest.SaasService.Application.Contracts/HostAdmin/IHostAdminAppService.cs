using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace ModuleTest.SaasService.HostAdmin;

/// <summary>
/// Host administration service
/// Note: Apply [AllowAnonymous] or [Authorize] at controller/method level
/// </summary>
public interface IHostAdminAppService : IApplicationService
{
    Task<HostMetricsDto> GetMetricsAsync();
    
    Task<PagedResultDto<TenantDetailDto>> GetTenantsAsync(PagedAndSortedResultRequestDto input);
    
    Task<TenantDetailDto> GetTenantDetailAsync(Guid tenantId);
    
    Task ActivateTenantAsync(Guid tenantId);
    
    Task DeactivateTenantAsync(Guid tenantId);
}
