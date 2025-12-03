using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace ModuleTest.SaasService.TenantAdmin;

/// <summary>
/// Tenant administration service
/// Note: Apply [AllowAnonymous] or [Authorize] at controller/method level
/// </summary>
public interface ITenantAdminAppService : IApplicationService
{
    Task<TenantDashboardDto> GetDashboardAsync();
}
