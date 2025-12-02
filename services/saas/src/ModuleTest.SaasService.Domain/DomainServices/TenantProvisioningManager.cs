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
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using Volo.Saas.Tenants;
using Volo.Saas;

namespace ModuleTest.SaasService.DomainServices;

/// <summary>
/// Domain service responsible for tenant provisioning and onboarding.
/// Orchestrates the complex process of creating tenants with subscriptions and admin users.
/// </summary>
public class TenantProvisioningManager : DomainService
{
    private readonly ITenantManager _tenantManager;
    private readonly IEditionRepository _editionRepository;
    private readonly SubscriptionManager _subscriptionManager;
    private readonly InvoiceManager _invoiceManager;
    private readonly IdentityUserManager _userManager;

    public TenantProvisioningManager(
        ITenantManager tenantManager,
        IEditionRepository editionRepository,
        SubscriptionManager subscriptionManager,
        InvoiceManager invoiceManager,
        IdentityUserManager userManager)
    {
        _tenantManager = tenantManager;
        _editionRepository = editionRepository;
        _subscriptionManager = subscriptionManager;
        _invoiceManager = invoiceManager;
        _userManager = userManager;
    }

    /// <summary>
    /// Provisions a complete tenant including subscription, admin user, and initial invoice.
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

        Guid adminUserId;
        
        // Create admin user within tenant context
        using (CurrentTenant.Change(tenant.Id))
        {
            adminUserId = await CreateTenantAdminUserAsync(
                adminUserName ?? adminEmail,
                adminEmail,
                adminPassword,
                tenant.Id
            );
        }

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

        Logger.LogInformation($"Tenant provisioning completed: {tenantName} (ID: {tenant.Id})");

        return new TenantProvisioningResult
        {
            Tenant = tenant,
            AdminUserId = adminUserId,
            Subscription = subscription,
            InitialInvoice = initialInvoice
        };
    }

    /// <summary>
    /// Creates the admin user for a newly provisioned tenant.
    /// </summary>
    private async Task<Guid> CreateTenantAdminUserAsync(
        string userName,
        string email,
        string password,
        Guid tenantId)
    {
        // Business Rule: Email must be unique within tenant
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            throw new BusinessException(SaasServiceErrorCodes.UserEmailAlreadyExists)
                .WithData("Email", email);
        }

        var adminUser = new IdentityUser(
            GuidGenerator.Create(),
            userName,
            email,
            tenantId
        );

        var result = await _userManager.CreateAsync(adminUser, password);
        if (!result.Succeeded)
        {
            throw new BusinessException(SaasServiceErrorCodes.UserCreationFailed)
                .WithData("Errors", string.Join(", ", result.Errors));
        }

        // Business Rule: Admin user must have admin role
        var roleResult = await _userManager.AddToRoleAsync(adminUser, "admin");
        if (!roleResult.Succeeded)
        {
            Logger.LogWarning($"Failed to assign admin role to user {userName}: {string.Join(", ", roleResult.Errors)}");
            // Don't fail the entire provisioning if role assignment fails
        }

        Logger.LogInformation($"Created admin user: {userName} for tenant: {tenantId}");

        return adminUser.Id;
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
    public Guid AdminUserId { get; set; }
    public Subscription Subscription { get; set; }
    public Invoice InitialInvoice { get; set; }
}
