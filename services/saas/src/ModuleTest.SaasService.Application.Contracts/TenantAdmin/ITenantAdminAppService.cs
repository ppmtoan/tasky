using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace ModuleTest.SaasService.TenantAdmin;

/// TODO: Remove [AllowAnonymous] in production and enforce proper authorization
/// This is temporarily disabled for testing purposes
[AllowAnonymous]
public interface ITenantAdminAppService : IApplicationService
{
    Task<TenantDashboardDto> GetDashboardAsync();
}
