using System;
using System.Linq.Expressions;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Enums;
using Volo.Abp.Specifications;

namespace ModuleTest.SaasService.Specifications;

/// <summary>
/// Specification for finding active subscriptions.
/// A subscription is active if its status is Active and it hasn't expired.
/// </summary>
public class ActiveSubscriptionsSpecification : Specification<Subscription>
{
    public override Expression<Func<Subscription, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        
        return subscription => 
            subscription.Status == SubscriptionStatus.Active &&
            subscription.SubscriptionPeriod.EndDate >= now;
    }
}

/// <summary>
/// Specification for finding active subscriptions for a specific tenant.
/// </summary>
public class TenantActiveSubscriptionSpecification : Specification<Subscription>
{
    private readonly Guid _tenantId;
    
    public TenantActiveSubscriptionSpecification(Guid tenantId)
    {
        _tenantId = tenantId;
    }
    
    public override Expression<Func<Subscription, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        
        return subscription => 
            subscription.TenantId == _tenantId &&
            subscription.Status == SubscriptionStatus.Active &&
            subscription.SubscriptionPeriod.EndDate >= now;
    }
}
