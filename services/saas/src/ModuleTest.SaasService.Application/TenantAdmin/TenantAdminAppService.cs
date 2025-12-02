using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.Repositories;
using ModuleTest.SaasService.ValueObjects;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Volo.Saas.Tenants;

namespace ModuleTest.SaasService.TenantAdmin;

public class TenantAdminAppService : ApplicationService, ITenantAdminAppService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IEditionRepository _editionRepository;

    public TenantAdminAppService(
        ITenantRepository tenantRepository,
        ISubscriptionRepository subscriptionRepository,
        IInvoiceRepository invoiceRepository,
        IIdentityUserRepository userRepository,
        IEditionRepository editionRepository)
    {
        _tenantRepository = tenantRepository;
        _subscriptionRepository = subscriptionRepository;
        _invoiceRepository = invoiceRepository;
        _userRepository = userRepository;
        _editionRepository = editionRepository;
    }

    public async Task<TenantDashboardDto> GetDashboardAsync()
    {
        if (!CurrentTenant.IsAvailable)
        {
            throw new BusinessException(SaasServiceErrorCodes.TenantNotAvailable);
        }

        var tenantId = CurrentTenant.Id.Value;
        var tenant = await _tenantRepository.GetAsync(tenantId);

        var subscriptionsQuery = await _subscriptionRepository.GetQueryableAsync();
        var subscription = await AsyncExecuter.FirstOrDefaultAsync(
            subscriptionsQuery
                .Where(s => s.TenantId == tenantId)
                .OrderByDescending(s => s.CreationTime)
        );

        if (subscription == null)
        {
            throw new BusinessException(SaasServiceErrorCodes.SubscriptionNotFound);
        }
        
        // Load the edition separately
        var edition = await _editionRepository.GetAsync(subscription.EditionId);

        var invoicesQuery = await _invoiceRepository.GetQueryableAsync();
        var invoices = await AsyncExecuter.ToListAsync(
            invoicesQuery.Where(i => i.TenantId == tenantId)
        );

        var userCount = (int)await _userRepository.GetCountAsync();

        var featureLimits = edition?.FeatureLimits != null
            ? new System.Collections.Generic.Dictionary<string, object>
            {
                ["MaxUsers"] = edition.FeatureLimits.MaxUsers,
                ["MaxProjects"] = edition.FeatureLimits.MaxProjects,
                ["StorageQuotaGB"] = edition.FeatureLimits.StorageQuotaGB,
                ["APICallsPerMonth"] = edition.FeatureLimits.APICallsPerMonth,
                ["EnableAdvancedReports"] = edition.FeatureLimits.EnableAdvancedReports,
                ["EnablePrioritySupport"] = edition.FeatureLimits.EnablePrioritySupport,
                ["EnableCustomBranding"] = edition.FeatureLimits.EnableCustomBranding
            }
            : new System.Collections.Generic.Dictionary<string, object>();

        return new TenantDashboardDto
        {
            TenantId = tenantId,
            TenantName = tenant.Name,
            EditionName = edition?.DisplayName,
            FeatureLimits = featureLimits,
            SubscriptionStatus = subscription.Status.ToString(),
            SubscriptionEndDate = subscription.SubscriptionPeriod.EndDate,
            NextBillingDate = subscription.NextBillingDate,
            DaysRemaining = subscription.SubscriptionPeriod.DaysRemaining(),
            CurrentPlanPrice = subscription.Price.Amount,
            BillingPeriod = subscription.BillingPeriod.ToString(),
            TotalUsers = userCount,
            PendingInvoices = invoices.Count(i => i.Status == InvoiceStatus.Pending),
            OverdueInvoices = invoices.Count(i => i.Status == InvoiceStatus.Overdue)
        };
    }
}
