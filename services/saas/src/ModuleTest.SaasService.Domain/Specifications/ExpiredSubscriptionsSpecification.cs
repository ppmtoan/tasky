using System;
using System.Linq.Expressions;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Enums;
using Volo.Abp.Specifications;

namespace ModuleTest.SaasService.Specifications;

/// <summary>
/// Specification for finding expired subscriptions.
/// A subscription is expired if its end date has passed or status is Expired.
/// </summary>
public class ExpiredSubscriptionsSpecification : Specification<Subscription>
{
    private readonly DateTime? _asOfDate;
    
    public ExpiredSubscriptionsSpecification(DateTime? asOfDate = null)
    {
        _asOfDate = asOfDate;
    }
    
    public override Expression<Func<Subscription, bool>> ToExpression()
    {
        var checkDate = _asOfDate ?? DateTime.UtcNow;
        
        return subscription => 
            (subscription.Status == SubscriptionStatus.Active && 
             subscription.SubscriptionPeriod.EndDate < checkDate) ||
            subscription.Status == SubscriptionStatus.Expired;
    }
}

/// <summary>
/// Specification for finding subscriptions expiring within a certain number of days.
/// </summary>
public class ExpiringSubscriptionsSpecification : Specification<Subscription>
{
    private readonly int _daysUntilExpiration;
    
    public ExpiringSubscriptionsSpecification(int daysUntilExpiration)
    {
        _daysUntilExpiration = daysUntilExpiration;
    }
    
    public override Expression<Func<Subscription, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        var expirationThreshold = now.AddDays(_daysUntilExpiration);
        
        return subscription => 
            subscription.Status == SubscriptionStatus.Active &&
            subscription.SubscriptionPeriod.EndDate > now &&
            subscription.SubscriptionPeriod.EndDate <= expirationThreshold;
    }
}

/// <summary>
/// Specification for finding trial subscriptions expiring within a certain number of days.
/// </summary>
public class TrialExpiringSpecification : Specification<Subscription>
{
    private readonly int _daysUntilExpiration;
    
    public TrialExpiringSpecification(int daysUntilExpiration)
    {
        _daysUntilExpiration = daysUntilExpiration;
    }
    
    public override Expression<Func<Subscription, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        var expirationThreshold = now.AddDays(_daysUntilExpiration);
        
        return subscription => 
            subscription.Status == SubscriptionStatus.Trial &&
            subscription.TrialEndDate.HasValue &&
            subscription.TrialEndDate.Value > now &&
            subscription.TrialEndDate.Value <= expirationThreshold;
    }
}
