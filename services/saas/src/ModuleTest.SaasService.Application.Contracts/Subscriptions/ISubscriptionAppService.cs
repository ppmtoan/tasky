using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ModuleTest.SaasService.Subscriptions;

public interface ISubscriptionAppService : ICrudAppService<SubscriptionDto, Guid, PagedAndSortedResultRequestDto, CreateSubscriptionDto, UpdateSubscriptionDto>
{
    Task<SubscriptionDto> GetCurrentTenantSubscriptionAsync();
    
    Task RenewAsync(Guid id);
    
    Task CancelAsync(Guid id);
    
    Task SuspendAsync(Guid id);
    
    Task ActivateAsync(Guid id);
}
