using System;
using System.Collections.Generic;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Events;
using ModuleTest.SaasService.ValueObjects;
using Volo.Abp.Domain.Entities.Auditing;

namespace ModuleTest.SaasService.Aggregates.EditionAggregate;

/// <summary>
/// Represents a Saas Edition (Plan) that defines feature limits and capabilities
/// Aggregate Root - controls all changes through methods
/// </summary>
public class Edition : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }
    
    public string DisplayName { get; private set; }
    
    public string Description { get; private set; }
    
    public Money MonthlyPrice { get; private set; }
    
    public Money YearlyPrice { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public int DisplayOrder { get; private set; }
    
    public FeatureLimits FeatureLimits { get; private set; }
    
    public ICollection<Subscription> Subscriptions { get; private set; }

    protected Edition()
    {
        Subscriptions = new List<Subscription>();
    }

    public Edition(
        Guid id,
        string name,
        string displayName,
        string description,
        Money monthlyPrice,
        Money yearlyPrice,
        FeatureLimits featureLimits,
        bool isActive = true,
        int displayOrder = 0) : base(id)
    {
        SetName(name);
        SetDisplayName(displayName);
        SetDescription(description);
        SetPricing(monthlyPrice, yearlyPrice);
        SetFeatureLimits(featureLimits);
        IsActive = isActive;
        DisplayOrder = displayOrder;
        Subscriptions = new List<Subscription>();
    }

    // Business methods with validation
    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Edition name cannot be empty", nameof(name));
        
        if (name.Length > 128)
            throw new ArgumentException("Edition name cannot exceed 128 characters", nameof(name));
        
        Name = name.Trim();
    }

    private void SetDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Edition display name cannot be empty", nameof(displayName));
        
        if (displayName.Length > 256)
            throw new ArgumentException("Edition display name cannot exceed 256 characters", nameof(displayName));
        
        DisplayName = displayName.Trim();
    }

    private void SetDescription(string description)
    {
        Description = description?.Trim();
    }

    private void SetPricing(Money monthlyPrice, Money yearlyPrice)
    {
        if (monthlyPrice == null)
            throw new ArgumentNullException(nameof(monthlyPrice));
        
        if (yearlyPrice == null)
            throw new ArgumentNullException(nameof(yearlyPrice));
        
        if (monthlyPrice.Amount < 0)
            throw new ArgumentException("Monthly price cannot be negative", nameof(monthlyPrice));
        
        if (yearlyPrice.Amount < 0)
            throw new ArgumentException("Yearly price cannot be negative", nameof(yearlyPrice));
        
        MonthlyPrice = monthlyPrice;
        YearlyPrice = yearlyPrice;
    }

    private void SetFeatureLimits(FeatureLimits featureLimits)
    {
        FeatureLimits = featureLimits ?? throw new ArgumentNullException(nameof(featureLimits));
    }

    public Money GetPriceForPeriod(Enums.BillingPeriod period)
    {
        return period == Enums.BillingPeriod.Yearly ? YearlyPrice : MonthlyPrice;
    }

    public void UpdatePricing(Money monthlyPrice, Money yearlyPrice)
    {
        SetPricing(monthlyPrice, yearlyPrice);
    }

    public void UpdateFeatureLimits(FeatureLimits newLimits)
    {
        SetFeatureLimits(newLimits);
    }

    public void UpdateDisplayName(string displayName)
    {
        SetDisplayName(displayName);
    }

    public void UpdateDescription(string description)
    {
        SetDescription(description);
    }

    public void Activate()
    {
        IsActive = true;
        
        AddDistributedEvent(new EditionActivatedEvent(
            editionId: Id,
            editionName: Name,
            isActivated: true
        ));
    }

    public void Deactivate()
    {
        IsActive = false;
        
        AddDistributedEvent(new EditionActivatedEvent(
            editionId: Id,
            editionName: Name,
            isActivated: false
        ));
    }

    public void UpdateDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative", nameof(displayOrder));
        
        DisplayOrder = displayOrder;
    }
}
