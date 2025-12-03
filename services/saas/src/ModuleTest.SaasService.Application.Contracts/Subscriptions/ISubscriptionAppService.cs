using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace ModuleTest.SaasService.Subscriptions;

/// TODO: Remove [AllowAnonymous] in production and enforce proper authorization
/// This is temporarily disabled for testing purposes
[AllowAnonymous]
public interface ISubscriptionAppService : ICrudAppService<SubscriptionDto, Guid, PagedAndSortedResultRequestDto, CreateSubscriptionDto, UpdateSubscriptionDto>
{
    Task<SubscriptionDto> GetCurrentTenantSubscriptionAsync();
    
    Task RenewAsync(Guid id);
    
    Task CancelAsync(Guid id);
    
    Task SuspendAsync(Guid id);
    
    Task ActivateAsync(Guid id);
}
