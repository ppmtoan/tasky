using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.Events;
using ModuleTest.SaasService.Repositories;
using ModuleTest.SaasService.ValueObjects;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using Volo.Saas.Tenants;
using Volo.Saas;

namespace ModuleTest.SaasService.DomainServices;

/// <summary>
/// Domain service responsible for tenant provisioning and onboarding.
/// Orchestrates the complex process of creating tenants with subscriptions.
/// Admin user creation is delegated to Identity service via distributed events.
/// </summary>
public class TenantProvisioningManager : DomainService
{
    private readonly ITenantManager _tenantManager;
    private readonly IEditionRepository _editionRepository;
    private readonly SubscriptionManager _subscriptionManager;
    private readonly InvoiceManager _invoiceManager;
    private readonly ILocalEventBus _localEventBus;

    public TenantProvisioningManager(
        ITenantManager tenantManager,
        IEditionRepository editionRepository,
        SubscriptionManager subscriptionManager,
        InvoiceManager invoiceManager,
        ILocalEventBus localEventBus)
    {
        _tenantManager = tenantManager;
        _editionRepository = editionRepository;
        _subscriptionManager = subscriptionManager;
        _invoiceManager = invoiceManager;
        _localEventBus = localEventBus;
    }

    /// <summary>
    /// Provisions a complete tenant including subscription and initial invoice.
    /// Raises event for Identity service to create admin user.
    /// This is a complex orchestration of multiple domain operations.
    /// </summary>
    [UnitOfWork]
    public virtual async Task<TenantProvisioningResult> ProvisionTenantAsync(
        string tenantName,
        Guid editionId,
        BillingPeriod billingPeriod,
        string adminEmail,
        string adminUserName,
        string adminPassword,
        int? trialDays = null)
    {
        Check.NotNullOrWhiteSpace(tenantName, nameof(tenantName));
        Check.NotNullOrWhiteSpace(adminEmail, nameof(adminEmail));
        Check.NotNullOrWhiteSpace(adminPassword, nameof(adminPassword));

        // Business Rule: Validate edition exists and is active
        var edition = await _editionRepository.GetAsync(editionId);
        if (!edition.IsActive)
        {
            throw new BusinessException(SaasServiceErrorCodes.EditionNotActive)
                .WithData("EditionId", editionId);
        }

        // Business Rule: Tenant name must be unique
        var tenant = await _tenantManager.CreateAsync(tenantName);
        
        Logger.LogInformation($"Created tenant: {tenantName} (ID: {tenant.Id})");

        // Create subscription
        var subscription = await _subscriptionManager.CreateSubscriptionAsync(
            tenant.Id,
            editionId,
            billingPeriod,
            Clock.Now,
            null, // Use edition price
            autoRenew: true,
            trialDays
        );

        Logger.LogInformation($"Created subscription for tenant: {tenant.Id}, Edition: {edition.DisplayName}");

        // Create initial invoice (only if not in trial)
        Invoice initialInvoice = null;
        if (!trialDays.HasValue || trialDays.Value == 0)
        {
            initialInvoice = await _invoiceManager.GenerateSubscriptionInvoiceAsync(
                subscription,
                1 // First invoice sequence
            );

            Logger.LogInformation($"Generated initial invoice for tenant: {tenant.Id}");
        }

        // Raise event for Identity service to create admin user
        // This decouples SaaS service from Identity service in microservices architecture
        await _localEventBus.PublishAsync(new TenantProvisionedEto
        {
            TenantId = tenant.Id,
            TenantName = tenantName,
            AdminEmail = adminEmail,
            AdminUserName = adminUserName ?? adminEmail,
            AdminPassword = adminPassword,
            EditionId = editionId,
            SubscriptionId = subscription.Id,
            IsTrialSubscription = trialDays.HasValue && trialDays.Value > 0,
            TrialDays = trialDays
        });

        Logger.LogInformation($"Tenant provisioning completed: {tenantName} (ID: {tenant.Id}). Admin user creation event raised.");

        return new TenantProvisioningResult
        {
            Tenant = tenant,
            Subscription = subscription,
            InitialInvoice = initialInvoice
        };
    }

    /// <summary>
    /// Deprovisions a tenant (soft delete with cleanup).
    /// </summary>
    [UnitOfWork]
    public virtual async Task DeprovisionTenantAsync(Guid tenantId, string reason)
    {
        Check.NotNull(tenantId, nameof(tenantId));
        Check.NotNullOrWhiteSpace(reason, nameof(reason));

        // Business Rule: Cancel active subscriptions before deprovisioning
        // This will be handled by the calling application service
        // Domain service focuses on the core business logic

        Logger.LogInformation($"Deprovisioning tenant: {tenantId}, Reason: {reason}");
    }
}

/// <summary>
/// Result object for tenant provisioning operation.
/// </summary>
public class TenantProvisioningResult
{
    public Tenant Tenant { get; set; }
    public Subscription Subscription { get; set; }
    public Invoice InitialInvoice { get; set; }
}
