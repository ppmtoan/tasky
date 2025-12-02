using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleTest.SaasService.Subscriptions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace ModuleTest.SaasService.Controllers.Subscriptions;

/// <summary>
/// Manages tenant subscriptions, billing cycles, and subscription lifecycle
/// </summary>
[RemoteService(Name = SaasServiceRemoteServiceConsts.RemoteServiceName)]
[Area(SaasServiceRemoteServiceConsts.ModuleName)]
[Route("api/saas/subscriptions")]
[AllowAnonymous] // For testing purposes - remove in production
public class SubscriptionController : AbpControllerBase, ISubscriptionAppService
{
    private readonly ISubscriptionAppService _subscriptionAppService;

    public SubscriptionController(ISubscriptionAppService subscriptionAppService)
    {
        _subscriptionAppService = subscriptionAppService;
    }

    /// <summary>
    /// Retrieves a paginated list of all subscriptions
    /// </summary>
    /// <param name="input">Pagination and sorting parameters</param>
    /// <returns>Paged list of subscriptions</returns>
    [HttpGet]
    public virtual Task<PagedResultDto<SubscriptionDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        return _subscriptionAppService.GetListAsync(input);
    }

    /// <summary>
    /// Retrieves a specific subscription by ID
    /// </summary>
    /// <param name="id">Subscription unique identifier</param>
    /// <returns>Subscription details</returns>
    [HttpGet]
    [Route("{id}")]
    public virtual Task<SubscriptionDto> GetAsync(Guid id)
    {
        return _subscriptionAppService.GetAsync(id);
    }

    /// <summary>
    /// Retrieves the current tenant's active subscription
    /// </summary>
    /// <returns>Current subscription details for the tenant</returns>
    [HttpGet]
    [Route("current")]
    public virtual Task<SubscriptionDto> GetCurrentTenantSubscriptionAsync()
    {
        return _subscriptionAppService.GetCurrentTenantSubscriptionAsync();
    }

    /// <summary>
    /// Creates a new subscription for a tenant
    /// </summary>
    /// <param name="input">Subscription details including edition, billing period, and pricing</param>
    /// <returns>The newly created subscription</returns>
    [HttpPost]
    public virtual Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto input)
    {
        return _subscriptionAppService.CreateAsync(input);
    }

    /// <summary>
    /// Updates an existing subscription
    /// </summary>
    /// <param name="id">Subscription ID</param>
    /// <param name="input">Updated subscription details</param>
    /// <returns>The updated subscription</returns>
    [HttpPut]
    [Route("{id}")]
    public virtual Task<SubscriptionDto> UpdateAsync(Guid id, UpdateSubscriptionDto input)
    {
        return _subscriptionAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// Deletes a subscription permanently
    /// </summary>
    /// <param name="id">Subscription ID</param>
    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _subscriptionAppService.DeleteAsync(id);
    }

    /// <summary>
    /// Renews an expiring or expired subscription for another billing cycle
    /// </summary>
    /// <param name="id">Subscription ID</param>
    [HttpPost]
    [Route("{id}/renew")]
    public virtual Task RenewAsync(Guid id)
    {
        return _subscriptionAppService.RenewAsync(id);
    }

    /// <summary>
    /// Cancels an active subscription (marks for cancellation at period end)
    /// </summary>
    /// <param name="id">Subscription ID</param>
    [HttpPost]
    [Route("{id}/cancel")]
    public virtual Task CancelAsync(Guid id)
    {
        return _subscriptionAppService.CancelAsync(id);
    }

    /// <summary>
    /// Suspends a subscription temporarily (e.g., for payment issues)
    /// </summary>
    /// <param name="id">Subscription ID</param>
    [HttpPost]
    [Route("{id}/suspend")]
    public virtual Task SuspendAsync(Guid id)
    {
        return _subscriptionAppService.SuspendAsync(id);
    }

    /// <summary>
    /// Activates a suspended subscription or trial
    /// </summary>
    /// <param name="id">Subscription ID</param>
    [HttpPost]
    [Route("{id}/activate")]
    public virtual Task ActivateAsync(Guid id)
    {
        return _subscriptionAppService.ActivateAsync(id);
    }
}
