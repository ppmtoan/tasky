using System;
using System.Linq;
using System.Threading.Tasks;
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
using Volo.Abp.TenantManagement;
using Volo.Saas.Tenants;

namespace ModuleTest.SaasService.HostAdmin;

public class HostAdminAppService : ApplicationService, IHostAdminAppService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IEditionRepository _editionRepository;
    private readonly ITenantManager _tenantManager;

    public HostAdminAppService(
        ITenantRepository tenantRepository,
        ISubscriptionRepository subscriptionRepository,
        IInvoiceRepository invoiceRepository,
        IEditionRepository editionRepository,
        ITenantManager tenantManager)
    {
        _tenantRepository = tenantRepository;
        _subscriptionRepository = subscriptionRepository;
        _invoiceRepository = invoiceRepository;
        _editionRepository = editionRepository;
        _tenantManager = tenantManager;
    }

    public async Task<HostMetricsDto> GetMetricsAsync()
    {
        // TODO: Re-enable permission check in production
        // await CheckPolicyAsync(SaasServicePermissions.Tenants.Default);

        var tenants = await _tenantRepository.GetListAsync();
        var subscriptionsQuery = await _subscriptionRepository.GetQueryableAsync();
        var subscriptions = await AsyncExecuter.ToListAsync(subscriptionsQuery);
        
        var invoicesQuery = await _invoiceRepository.GetQueryableAsync();
        var invoices = await AsyncExecuter.ToListAsync(invoicesQuery);

        var activeSubscriptions = subscriptions.Where(s => s.Status == SubscriptionStatus.Active).ToList();
        var trialSubscriptions = subscriptions.Where(s => s.Status == SubscriptionStatus.Trial).ToList();
        var expiredSubscriptions = subscriptions.Where(s => s.Status == SubscriptionStatus.Expired).ToList();
        var suspendedSubscriptions = subscriptions.Where(s => s.Status == SubscriptionStatus.Suspended).ToList();

        var mrr = activeSubscriptions
            .Where(s => s.BillingPeriod == BillingPeriod.Monthly)
            .Sum(s => s.Price.Amount);

        var arr = activeSubscriptions
            .Where(s => s.BillingPeriod == BillingPeriod.Yearly)
            .Sum(s => s.Price.Amount);

        var totalRevenue = invoices
            .Where(i => i.Status == InvoiceStatus.Paid)
            .Sum(i => i.Amount.Amount);

        return new HostMetricsDto
        {
            TotalTenants = tenants.Count,
            ActiveTenants = activeSubscriptions.Select(s => s.TenantId).Distinct().Count(),
            TrialTenants = trialSubscriptions.Select(s => s.TenantId).Distinct().Count(),
            ExpiredTenants = expiredSubscriptions.Select(s => s.TenantId).Distinct().Count(),
            SuspendedTenants = suspendedSubscriptions.Select(s => s.TenantId).Distinct().Count(),
            MonthlyRecurringRevenue = mrr,
            YearlyRecurringRevenue = arr,
            TotalSubscriptions = subscriptions.Count,
            PendingInvoices = invoices.Count(i => i.Status == InvoiceStatus.Pending),
            OverdueInvoices = invoices.Count(i => i.Status == InvoiceStatus.Overdue),
            TotalRevenue = totalRevenue
        };
    }

    public async Task<PagedResultDto<TenantDetailDto>> GetTenantsAsync(PagedAndSortedResultRequestDto input)
    {
        // TODO: Re-enable permission check in production
        // await CheckPolicyAsync(SaasServicePermissions.Tenants.Default);

        var tenants = await _tenantRepository.GetListAsync(
            skipCount: input.SkipCount,
            maxResultCount: input.MaxResultCount
        );
        var totalCount = await _tenantRepository.GetCountAsync();

        var tenantIds = tenants.Select(t => t.Id).ToList();
        var subscriptionsQuery = await _subscriptionRepository.GetQueryableAsync();
        var subscriptions = await AsyncExecuter.ToListAsync(
            subscriptionsQuery.Where(s => tenantIds.Contains(s.TenantId.Value))
        );

        // Load all editions for subscriptions
        var editionIds = subscriptions.Select(s => s.EditionId).Distinct().ToList();
        var editionsQuery = await _editionRepository.GetQueryableAsync();
        var editions = await AsyncExecuter.ToListAsync(
            editionsQuery.Where(e => editionIds.Contains(e.Id))
        );

        var invoicesQuery = await _invoiceRepository.GetQueryableAsync();
        var invoices = await AsyncExecuter.ToListAsync(
            invoicesQuery.Where(i => tenantIds.Contains(i.TenantId.Value) && i.Status == InvoiceStatus.Paid)
        );

        var tenantDetails = tenants.Select(tenant =>
        {
            var subscription = subscriptions
                .Where(s => s.TenantId == tenant.Id)
                .OrderByDescending(s => s.CreationTime)
                .FirstOrDefault();

            var edition = subscription != null 
                ? editions.FirstOrDefault(e => e.Id == subscription.EditionId)
                : null;

            var revenue = invoices
                .Where(i => i.TenantId == tenant.Id)
                .Sum(i => i.Amount.Amount);

            return new TenantDetailDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                IsActive = !tenant.IsDeleted,
                CreationTime = tenant.CreationTime,
                EditionName = edition?.DisplayName,
                SubscriptionStatus = subscription?.Status.ToString(),
                SubscriptionEndDate = subscription?.SubscriptionPeriod.EndDate,
                TotalRevenue = revenue,
                ConnectionStrings = tenant.ConnectionStrings?.Select(cs => cs.Name).ToList() ?? new System.Collections.Generic.List<string>()
            };
        }).ToList();

        return new PagedResultDto<TenantDetailDto>(totalCount, tenantDetails);
    }

    public async Task<TenantDetailDto> GetTenantDetailAsync(Guid tenantId)
    {
        // TODO: Re-enable permission check in production
        // await CheckPolicyAsync(SaasServicePermissions.Tenants.Default);

        var tenant = await _tenantRepository.GetAsync(tenantId);
        
        var subscriptionsQuery = await _subscriptionRepository.GetQueryableAsync();
        var subscription = await AsyncExecuter.FirstOrDefaultAsync(
            subscriptionsQuery
                .Where(s => s.TenantId == tenantId)
                .OrderByDescending(s => s.CreationTime)
        );

        Edition edition = null;
        if (subscription != null)
        {
            edition = await _editionRepository.GetAsync(subscription.EditionId);
        }

        var invoicesQuery = await _invoiceRepository.GetQueryableAsync();
        var paidInvoices = await AsyncExecuter.ToListAsync(
            invoicesQuery.Where(i => i.TenantId == tenantId && i.Status == InvoiceStatus.Paid)
        );
        var revenue = paidInvoices.Sum(i => i.Amount.Amount);

        return new TenantDetailDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            IsActive = !tenant.IsDeleted,
            CreationTime = tenant.CreationTime,
            EditionName = edition?.DisplayName,
            SubscriptionStatus = subscription?.Status.ToString(),
            SubscriptionEndDate = subscription?.SubscriptionPeriod.EndDate,
            TotalRevenue = revenue,
            ConnectionStrings = tenant.ConnectionStrings?.Select(cs => cs.Name).ToList() ?? new System.Collections.Generic.List<string>()
        };
    }

    public async Task ActivateTenantAsync(Guid tenantId)
    {
        // TODO: Re-enable permission check in production
        // await CheckPolicyAsync(SaasServicePermissions.Tenants.Update);

        var tenant = await _tenantRepository.GetAsync(tenantId);
        // Tenant is active by default when not deleted
        if (tenant.IsDeleted)
        {
            // Cannot undelete via this method - would need special handling
            throw new BusinessException(SaasServiceErrorCodes.TenantNotAvailable)
                .WithData("TenantId", tenantId);
        }
        
        await _tenantRepository.UpdateAsync(tenant);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task DeactivateTenantAsync(Guid tenantId)
    {
        // TODO: Re-enable permission check in production
        // await CheckPolicyAsync(SaasServicePermissions.Tenants.Update);

        var tenant = await _tenantRepository.GetAsync(tenantId);
        // Soft delete to deactivate
        await _tenantRepository.DeleteAsync(tenant, autoSave: true);
    }
}
