using System;

namespace ModuleTest.SaasService.Events;

/// <summary>
/// Event Transfer Object (ETO) for distributed event when tenant is provisioned.
/// This event is consumed by Identity service to create admin user.
/// </summary>
[Serializable]
public class TenantProvisionedEto
{
    public Guid TenantId { get; set; }
    
    public string TenantName { get; set; }
    
    public Guid SubscriptionId { get; set; }
    
    public Guid EditionId { get; set; }
    
    /// <summary>
    /// Admin user email for the new tenant
    /// </summary>
    public string AdminEmail { get; set; }
    
    /// <summary>
    /// Admin username for the new tenant
    /// </summary>
    public string AdminUserName { get; set; }
    
    /// <summary>
    /// Admin password (will be hashed by Identity service)
    /// </summary>
    public string AdminPassword { get; set; }
    
    public bool IsTrialSubscription { get; set; }
    
    public int? TrialDays { get; set; }
    
    public DateTime ProvisionedAt { get; set; } = DateTime.UtcNow;
}
