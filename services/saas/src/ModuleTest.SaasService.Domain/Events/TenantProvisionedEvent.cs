using System;

namespace ModuleTest.SaasService.Events;

[Serializable]
public class TenantProvisionedEvent
{
    public Guid TenantId { get; set; }
    
    public Guid SubscriptionId { get; set; }
    
    public Guid EditionId { get; set; }
    
    public string TenantName { get; set; }
    
    public bool IsTrialSubscription { get; set; }
    
    public int? TrialDays { get; set; }
    
    public DateTime ProvisionedAt { get; set; }

    public TenantProvisionedEvent(
        Guid tenantId,
        Guid subscriptionId,
        Guid editionId,
        string tenantName,
        bool isTrialSubscription,
        int? trialDays)
    {
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        EditionId = editionId;
        TenantName = tenantName;
        IsTrialSubscription = isTrialSubscription;
        TrialDays = trialDays;
        ProvisionedAt = DateTime.UtcNow;
    }
}
