using System;
using ModuleTest.SaasService.Enums;

namespace ModuleTest.SaasService.Subscriptions;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    
    public Guid? TenantId { get; set; }
    
    public string TenantName { get; set; }
    
    public Guid EditionId { get; set; }
    
    public string EditionName { get; set; }
    
    public BillingPeriod BillingPeriod { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public DateTime? NextBillingDate { get; set; }
    
    public bool AutoRenew { get; set; }
    
    public decimal Price { get; set; }
    
    public SubscriptionStatus Status { get; set; }
    
    public int? TrialDays { get; set; }
    
    public DateTime? TrialEndDate { get; set; }
    
    public int DaysRemaining { get; set; }
}
