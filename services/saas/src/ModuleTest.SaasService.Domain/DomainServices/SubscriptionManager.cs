using System;
using System.Threading.Tasks;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.Repositories;
using ModuleTest.SaasService.Specifications;
using ModuleTest.SaasService.ValueObjects;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Saas;

namespace ModuleTest.SaasService.DomainServices;

/// <summary>
/// Domain service responsible for subscription-related business logic.
/// Encapsulates complex subscription operations and business rules.
/// </summary>
public class SubscriptionManager : DomainService
{
    private readonly IEditionRepository _editionRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public SubscriptionManager(
        IEditionRepository editionRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _editionRepository = editionRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    /// <summary>
    /// Creates a new subscription for a tenant with proper validation and pricing.
    /// </summary>
    public async Task<Subscription> CreateSubscriptionAsync(
        Guid tenantId,
        Guid editionId,
        BillingPeriod billingPeriod,
        DateTime? startDate = null,
        Money customPrice = null,
        bool autoRenew = true,
        int? trialDays = null)
    {
        // Business Rule: Edition must exist and be active
        var edition = await _editionRepository.GetAsync(editionId);
        if (edition == null || !edition.IsActive)
        {
            throw new BusinessException(SaasServiceErrorCodes.EditionNotFound)
                .WithData("EditionId", editionId);
        }

        // Business Rule: Tenant can only have one active subscription
        var existingSubscription = await _subscriptionRepository.FindActiveByTenantIdAsync(tenantId);
        if (existingSubscription != null)
        {
            throw new BusinessException(SaasServiceErrorCodes.TenantAlreadyHasActiveSubscription)
                .WithData("TenantId", tenantId)
                .WithData("ExistingSubscriptionId", existingSubscription.Id);
        }

        // Business Rule: Use custom price if provided, otherwise use edition price
        var price = customPrice ?? edition.GetPriceForPeriod(billingPeriod);

        // Business Rule: Validate pricing (no zero or negative prices unless trial)
        if (price.IsZero() && (!trialDays.HasValue || trialDays.Value == 0))
        {
            throw new BusinessException(SaasServiceErrorCodes.InvalidSubscriptionPrice)
                .WithData("Price", price.Amount);
        }

        var subscription = new Subscription(
            GuidGenerator.Create(),
            tenantId,
            editionId,
            billingPeriod,
            startDate ?? Clock.Now,
            price,
            autoRenew,
            trialDays
        );

        return subscription;
    }

    /// <summary>
    /// Renews an existing subscription, handling price changes and billing periods.
    /// </summary>
    public async Task RenewSubscriptionAsync(Subscription subscription)
    {
        Check.NotNull(subscription, nameof(subscription));

        // Business Rule: Can only renew active or expired subscriptions
        if (subscription.Status != SubscriptionStatus.Active && 
            subscription.Status != SubscriptionStatus.Expired)
        {
            throw new BusinessException(SaasServiceErrorCodes.CannotRenewSubscription)
                .WithData("SubscriptionId", subscription.Id)
                .WithData("Status", subscription.Status);
        }

        // Business Rule: Fetch latest edition pricing
        var edition = await _editionRepository.GetAsync(subscription.EditionId);
        if (!edition.IsActive)
        {
            throw new BusinessException(SaasServiceErrorCodes.EditionNotActive)
                .WithData("EditionId", edition.Id);
        }

        var newPrice = edition.GetPriceForPeriod(subscription.BillingPeriod);
        subscription.Renew(newPrice);
    }

    /// <summary>
    /// Upgrades a subscription to a new edition with prorated pricing.
    /// </summary>
    public async Task<(Subscription newSubscription, Money proratedCredit)> UpgradeSubscriptionAsync(
        Subscription currentSubscription,
        Guid newEditionId,
        DateTime upgradeDate)
    {
        Check.NotNull(currentSubscription, nameof(currentSubscription));

        // Business Rule: Can only upgrade active subscriptions
        if (currentSubscription.Status != SubscriptionStatus.Active)
        {
            throw new BusinessException(SaasServiceErrorCodes.CannotUpgradeInactiveSubscription)
                .WithData("SubscriptionId", currentSubscription.Id)
                .WithData("Status", currentSubscription.Status);
        }

        var newEdition = await _editionRepository.GetAsync(newEditionId);
        if (!newEdition.IsActive)
        {
            throw new BusinessException(SaasServiceErrorCodes.EditionNotActive)
                .WithData("EditionId", newEditionId);
        }

        // Business Rule: Calculate prorated credit from unused time
        var remainingDays = currentSubscription.SubscriptionPeriod.DaysRemaining();
        var totalDays = currentSubscription.BillingPeriod == BillingPeriod.Monthly ? 30 : 365;
        var unusedRatio = (decimal)remainingDays / totalDays;
        var proratedCredit = currentSubscription.Price.Multiply(unusedRatio);

        // Cancel current subscription
        currentSubscription.Cancel();

        // Create new subscription with prorated start
        var newPrice = newEdition.GetPriceForPeriod(currentSubscription.BillingPeriod);
        var adjustedPrice = newPrice.Subtract(proratedCredit);

        var newSubscription = new Subscription(
            GuidGenerator.Create(),
            currentSubscription.TenantId,
            newEditionId,
            currentSubscription.BillingPeriod,
            upgradeDate,
            adjustedPrice.Amount > 0 ? adjustedPrice : new Money(0),
            currentSubscription.AutoRenew,
            null
        );

        return (newSubscription, proratedCredit);
    }

    /// <summary>
    /// Downgrades a subscription to a lower edition.
    /// </summary>
    public async Task<Subscription> DowngradeSubscriptionAsync(
        Subscription currentSubscription,
        Guid newEditionId)
    {
        Check.NotNull(currentSubscription, nameof(currentSubscription));

        // Business Rule: Can only downgrade active subscriptions
        if (currentSubscription.Status != SubscriptionStatus.Active)
        {
            throw new BusinessException(SaasServiceErrorCodes.CannotDowngradeInactiveSubscription)
                .WithData("SubscriptionId", currentSubscription.Id)
                .WithData("Status", currentSubscription.Status);
        }

        var newEdition = await _editionRepository.GetAsync(newEditionId);
        if (!newEdition.IsActive)
        {
            throw new BusinessException(SaasServiceErrorCodes.EditionNotActive)
                .WithData("EditionId", newEditionId);
        }

        // Business Rule: Downgrade takes effect at end of current billing period
        currentSubscription.DisableAutoRenew();

        // Create new subscription starting when current one ends
        var newPrice = newEdition.GetPriceForPeriod(currentSubscription.BillingPeriod);
        var newSubscription = new Subscription(
            GuidGenerator.Create(),
            currentSubscription.TenantId,
            newEditionId,
            currentSubscription.BillingPeriod,
            currentSubscription.SubscriptionPeriod.EndDate,
            newPrice,
            false, // Don't auto-renew the downgraded subscription initially
            null
        );

        return newSubscription;
    }

    /// <summary>
    /// Suspends a subscription for non-payment or policy violation.
    /// </summary>
    public void SuspendSubscription(Subscription subscription, string reason)
    {
        Check.NotNull(subscription, nameof(subscription));
        Check.NotNullOrWhiteSpace(reason, nameof(reason));

        // Business Rule: Can only suspend active subscriptions
        if (subscription.Status != SubscriptionStatus.Active)
        {
            throw new BusinessException(SaasServiceErrorCodes.CannotSuspendInactiveSubscription)
                .WithData("SubscriptionId", subscription.Id)
                .WithData("Status", subscription.Status);
        }

        subscription.Suspend();
    }

    /// <summary>
    /// Validates if a subscription can perform an action based on feature limits.
    /// </summary>
    public async Task<bool> ValidateFeatureLimitAsync(
        Subscription subscription,
        string featureName,
        int currentUsage)
    {
        var edition = await _editionRepository.GetAsync(subscription.EditionId);
        
        return featureName switch
        {
            "MaxUsers" => currentUsage < edition.FeatureLimits.MaxUsers,
            "MaxProjects" => currentUsage < edition.FeatureLimits.MaxProjects,
            _ => true
        };
    }
}
