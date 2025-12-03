using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace ModuleTest.SaasService.HostAdmin;

/// TODO: Remove [AllowAnonymous] in production and enforce proper authorization
/// This is temporarily disabled for testing purposes
[AllowAnonymous]
public interface IHostAdminAppService : IApplicationService
{
    Task<HostMetricsDto> GetMetricsAsync();
    
    Task<PagedResultDto<TenantDetailDto>> GetTenantsAsync(PagedAndSortedResultRequestDto input);
    
    Task<TenantDetailDto> GetTenantDetailAsync(Guid tenantId);
    
    Task ActivateTenantAsync(Guid tenantId);
    
    Task DeactivateTenantAsync(Guid tenantId);
}
