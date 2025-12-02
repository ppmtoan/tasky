using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ModuleTest.SaasService.TenantAdmin;

public interface ITenantAdminAppService : IApplicationService
{
    Task<TenantDashboardDto> GetDashboardAsync();
}
