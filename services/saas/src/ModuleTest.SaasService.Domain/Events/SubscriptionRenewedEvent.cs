using System;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.ValueObjects;

namespace ModuleTest.SaasService.Events;

[Serializable]
public class SubscriptionRenewedEvent
{
    public Guid SubscriptionId { get; set; }
    
    public Guid? TenantId { get; set; }
    
    public Guid EditionId { get; set; }
    
    public DateTime OldEndDate { get; set; }
    
    public DateTime NewEndDate { get; set; }
    
    public Money Price { get; set; }
    
    public BillingPeriod BillingPeriod { get; set; }
    
    public DateTime RenewedAt { get; set; }

    public SubscriptionRenewedEvent(
        Guid subscriptionId,
        Guid? tenantId,
        Guid editionId,
        DateTime oldEndDate,
        DateTime newEndDate,
        Money price,
        BillingPeriod billingPeriod)
    {
        SubscriptionId = subscriptionId;
        TenantId = tenantId;
        EditionId = editionId;
        OldEndDate = oldEndDate;
        NewEndDate = newEndDate;
        Price = price;
        BillingPeriod = billingPeriod;
        RenewedAt = DateTime.UtcNow;
    }
}
