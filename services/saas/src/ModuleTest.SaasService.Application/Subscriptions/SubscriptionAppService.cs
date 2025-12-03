using System;
using System.Linq;
using System.Threading.Tasks;
using ModuleTest.SaasService.DomainServices;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.Permissions;
using ModuleTest.SaasService.Repositories;
using ModuleTest.SaasService.ValueObjects;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ModuleTest.SaasService.Subscriptions;

public class SubscriptionAppService : CrudAppService<Subscription, SubscriptionDto, Guid, PagedAndSortedResultRequestDto, CreateSubscriptionDto, UpdateSubscriptionDto>, ISubscriptionAppService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IEditionRepository _editionRepository;
    private readonly SubscriptionManager _subscriptionManager;

    public SubscriptionAppService(
        ISubscriptionRepository repository,
        IEditionRepository editionRepository,
        SubscriptionManager subscriptionManager) : base(repository)
    {
        _subscriptionRepository = repository;
        _editionRepository = editionRepository;
        _subscriptionManager = subscriptionManager;
        
        // TODO: Re-enable permissions in production
        // GetPolicyName = SaasServicePermissions.Subscriptions.Default;
        // GetListPolicyName = SaasServicePermissions.Subscriptions.Default;
        // CreatePolicyName = SaasServicePermissions.Subscriptions.Create;
        // UpdatePolicyName = SaasServicePermissions.Subscriptions.Update;
        // DeletePolicyName = SaasServicePermissions.Subscriptions.Delete;
    }

    public override async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto input)
    {
        // Delegate business logic to domain service
        var subscription = await _subscriptionManager.CreateSubscriptionAsync(
            input.TenantId ?? CurrentTenant.Id.Value,
            input.EditionId,
            input.BillingPeriod,
            input.StartDate,
            input.Price > 0 ? new Money(input.Price) : null,
            input.AutoRenew,
            input.TrialDays
        );

        await _subscriptionRepository.InsertAsync(subscription);
        await CurrentUnitOfWork.SaveChangesAsync();

        return await MapToGetOutputDtoAsync(subscription);
    }

    public override async Task<SubscriptionDto> UpdateAsync(Guid id, UpdateSubscriptionDto input)
    {
        await CheckUpdatePolicyAsync();

        var subscription = await GetEntityByIdAsync(id);
        
        subscription.UpdateBillingPeriod(input.BillingPeriod, new Money(input.Price));
        
        if (input.AutoRenew)
        {
            subscription.EnableAutoRenew();
        }
        else
        {
            subscription.DisableAutoRenew();
        }

        await _subscriptionRepository.UpdateAsync(subscription);
        await CurrentUnitOfWork.SaveChangesAsync();

        return await MapToGetOutputDtoAsync(subscription);
    }

    public async Task<SubscriptionDto> GetCurrentTenantSubscriptionAsync()
    {
        if (!CurrentTenant.IsAvailable)
        {
            throw new BusinessException(SaasServiceErrorCodes.TenantNotAvailable);
        }

        var query = await _subscriptionRepository.GetQueryableAsync();
        var subscription = await AsyncExecuter.FirstOrDefaultAsync(
            query.Where(s => s.TenantId == CurrentTenant.Id)
                .OrderByDescending(s => s.CreationTime)
        );

        if (subscription == null)
        {
            throw new BusinessException(SaasServiceErrorCodes.SubscriptionNotFound);
        }

        return await MapToGetOutputDtoAsync(subscription);
    }

    public async Task RenewAsync(Guid id)
    {
        await CheckPolicyAsync(SaasServicePermissions.Subscriptions.Manage);

        var subscription = await GetEntityByIdAsync(id);
        
        // Delegate business logic to domain service
        await _subscriptionManager.RenewSubscriptionAsync(subscription);

        await _subscriptionRepository.UpdateAsync(subscription);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task CancelAsync(Guid id)
    {
        await CheckPolicyAsync(SaasServicePermissions.Subscriptions.Manage);

        var subscription = await GetEntityByIdAsync(id);
        subscription.Cancel();

        await _subscriptionRepository.UpdateAsync(subscription);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task SuspendAsync(Guid id)
    {
        await CheckPolicyAsync(SaasServicePermissions.Subscriptions.Manage);

        var subscription = await GetEntityByIdAsync(id);
        
        // Delegate business logic to domain service
        _subscriptionManager.SuspendSubscription(subscription, "Suspended by administrator");

        await _subscriptionRepository.UpdateAsync(subscription);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task ActivateAsync(Guid id)
    {
        await CheckPolicyAsync(SaasServicePermissions.Subscriptions.Manage);

        var subscription = await GetEntityByIdAsync(id);
        subscription.Activate();

        await _subscriptionRepository.UpdateAsync(subscription);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    protected override async Task<IQueryable<Subscription>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
    {
        return await base.CreateFilteredQueryAsync(input);
    }

    protected override IQueryable<Subscription> ApplyDefaultSorting(IQueryable<Subscription> query)
    {
        return query.OrderByDescending(s => s.CreationTime);
    }
}
