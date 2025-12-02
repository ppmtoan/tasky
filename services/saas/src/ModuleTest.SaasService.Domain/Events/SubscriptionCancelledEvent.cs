using System;

namespace ModuleTest.SaasService.Events;

[Serializable]
public class SubscriptionCancelledEvent
{
    public Guid SubscriptionId { get; set; }
    
    public Guid? TenantId { get; set; }
    
    public Guid EditionId { get; set; }
    
    public DateTime SubscriptionEndDate { get; set; }
    
    public DateTime CancelledAt { get; set; }
    
    public string CancellationReason { get; set; }

    public SubscriptionCancelledEvent(
        Guid subscriptionId,
        Guid? tenantId,
        Guid editionId,
        DateTime subscriptionEndDate,
        string cancellationReason = null)
    {
        SubscriptionId = subscriptionId;
        TenantId = tenantId;
        EditionId = editionId;
        SubscriptionEndDate = subscriptionEndDate;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = cancellationReason;
    }
}
