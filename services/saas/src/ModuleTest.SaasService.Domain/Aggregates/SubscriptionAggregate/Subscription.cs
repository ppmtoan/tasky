using System;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.Events;
using ModuleTest.SaasService.ValueObjects;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ModuleTest.SaasService.Aggregates.SubscriptionAggregate;

/// <summary>
/// Represents a tenant's subscription to an edition with billing information
/// Aggregate Root - controls all changes through methods
/// </summary>
public class Subscription : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    
    public Guid EditionId { get; private set; }
    
    public Edition Edition { get; private set; }
    
    public BillingPeriod BillingPeriod { get; private set; }
    
    public DateRange SubscriptionPeriod { get; private set; }
    
    public DateTime? NextBillingDate { get; private set; }
    
    public bool AutoRenew { get; private set; }
    
    public Money Price { get; private set; }
    
    public SubscriptionStatus Status { get; private set; }
    
    public int? TrialDays { get; private set; }
    
    public DateTime? TrialEndDate { get; private set; }

    protected Subscription()
    {
    }

    public Subscription(
        Guid id,
        Guid? tenantId,
        Guid editionId,
        BillingPeriod billingPeriod,
        DateTime startDate,
        Money price,
        bool autoRenew = true,
        int? trialDays = null) : base(id)
    {
        if (editionId == Guid.Empty)
            throw new ArgumentException("Edition ID cannot be empty", nameof(editionId));
        
        if (price == null)
            throw new ArgumentNullException(nameof(price));
        
        if (trialDays.HasValue && trialDays.Value < 0)
            throw new ArgumentException("Trial days cannot be negative", nameof(trialDays));
        
        TenantId = tenantId;
        EditionId = editionId;
        BillingPeriod = billingPeriod;
        Price = price;
        AutoRenew = autoRenew;
        TrialDays = trialDays;
        
        if (trialDays.HasValue && trialDays.Value > 0)
        {
            Status = SubscriptionStatus.Trial;
            TrialEndDate = startDate.AddDays(trialDays.Value);
            SubscriptionPeriod = new DateRange(startDate, TrialEndDate.Value);
        }
        else
        {
            Status = SubscriptionStatus.Active;
            var endDate = CalculateEndDate(startDate, billingPeriod);
            SubscriptionPeriod = new DateRange(startDate, endDate);
        }
        
        NextBillingDate = CalculateNextBillingDate();
    }

    public void Renew()
    {
        if (Status == SubscriptionStatus.Trial && TrialEndDate.HasValue)
        {
            // Transition from trial to active
            Status = SubscriptionStatus.Active;
            var endDate = CalculateEndDate(TrialEndDate.Value, BillingPeriod);
            SubscriptionPeriod = new DateRange(TrialEndDate.Value, endDate);
        }
        else
        {
            SubscriptionPeriod = BillingPeriod == BillingPeriod.Yearly 
                ? SubscriptionPeriod.ExtendByYears(1)
                : SubscriptionPeriod.ExtendByMonths(1);
        }
        
        NextBillingDate = CalculateNextBillingDate();
    }

    public void Renew(Money newPrice)
    {
        Renew();
        Price = newPrice;
    }

    public void Cancel()
    {
        ChangeStatus(SubscriptionStatus.Cancelled);
        AutoRenew = false;
        NextBillingDate = null;
        
        AddDistributedEvent(new SubscriptionCancelledEvent(
            subscriptionId: Id,
            tenantId: TenantId,
            editionId: EditionId,
            subscriptionEndDate: SubscriptionPeriod.EndDate
        ));
    }

    public void Suspend()
    {
        ChangeStatus(SubscriptionStatus.Suspended);
    }

    public void Activate()
    {
        ChangeStatus(SubscriptionStatus.Active);
    }

    public void MarkAsExpired()
    {
        ChangeStatus(SubscriptionStatus.Expired);
        AutoRenew = false;
    }

    public bool IsActive()
    {
        return Status == SubscriptionStatus.Active && SubscriptionPeriod.IsActive();
    }

    public int DaysRemaining()
    {
        return SubscriptionPeriod.DaysRemaining();
    }

    private DateTime CalculateEndDate(DateTime startDate, BillingPeriod period)
    {
        return period switch
        {
            BillingPeriod.Monthly => startDate.AddMonths(1),
            BillingPeriod.Yearly => startDate.AddYears(1),
            _ => startDate.AddMonths(1)
        };
    }

    private DateTime? CalculateNextBillingDate()
    {
        if (!AutoRenew || Status == SubscriptionStatus.Cancelled)
        {
            return null;
        }

        return SubscriptionPeriod.EndDate;
    }

    public void UpdateBillingPeriod(BillingPeriod newPeriod, Money newPrice)
    {
        if (newPrice == null)
            throw new ArgumentNullException(nameof(newPrice));
        
        BillingPeriod = newPeriod;
        Price = newPrice;
        var endDate = CalculateEndDate(SubscriptionPeriod.StartDate, newPeriod);
        SubscriptionPeriod = new DateRange(SubscriptionPeriod.StartDate, endDate);
        NextBillingDate = CalculateNextBillingDate();
    }

    public void EnableAutoRenew()
    {
        if (Status == SubscriptionStatus.Cancelled)
            throw new InvalidOperationException("Cannot enable auto-renew for cancelled subscription");
        
        AutoRenew = true;
        NextBillingDate = CalculateNextBillingDate();
    }

    public void DisableAutoRenew()
    {
        AutoRenew = false;
        NextBillingDate = null;
    }

    /// <summary>
    /// Changes subscription status with state machine validation.
    /// </summary>
    private void ChangeStatus(SubscriptionStatus newStatus)
    {
        SubscriptionStateMachine.ValidateTransition(Status, newStatus);
        Status = newStatus;
    }
}
