using System;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ModuleTest.SaasService.Subscriptions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace ModuleTest.SaasService.Controllers.Subscriptions;

[RemoteService(Name = SaasServiceRemoteServiceConsts.RemoteServiceName)]
[Area(SaasServiceRemoteServiceConsts.ModuleName)]
[Route("api/saas-service/subscriptions")]
public class SubscriptionController : AbpControllerBase, ISubscriptionAppService
{
    private readonly ISubscriptionAppService _subscriptionAppService;

    public SubscriptionController(ISubscriptionAppService subscriptionAppService)
    {
        _subscriptionAppService = subscriptionAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<SubscriptionDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        return _subscriptionAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<SubscriptionDto> GetAsync(Guid id)
    {
        return _subscriptionAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("current")]
    public virtual Task<SubscriptionDto> GetCurrentTenantSubscriptionAsync()
    {
        return _subscriptionAppService.GetCurrentTenantSubscriptionAsync();
    }

    [HttpPost]
    public virtual Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto input)
    {
        return _subscriptionAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<SubscriptionDto> UpdateAsync(Guid id, UpdateSubscriptionDto input)
    {
        return _subscriptionAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _subscriptionAppService.DeleteAsync(id);
    }

    [HttpPost]
    [Route("{id}/renew")]
    public virtual Task RenewAsync(Guid id)
    {
        return _subscriptionAppService.RenewAsync(id);
    }

    [HttpPost]
    [Route("{id}/cancel")]
    public virtual Task CancelAsync(Guid id)
    {
        return _subscriptionAppService.CancelAsync(id);
    }

    [HttpPost]
    [Route("{id}/suspend")]
    public virtual Task SuspendAsync(Guid id)
    {
        return _subscriptionAppService.SuspendAsync(id);
    }

    [HttpPost]
    [Route("{id}/activate")]
    public virtual Task ActivateAsync(Guid id)
    {
        return _subscriptionAppService.ActivateAsync(id);
    }
}
