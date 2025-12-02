using System;
using System.Collections.Generic;

namespace ModuleTest.SaasService.TenantAdmin;

public class TenantDashboardDto
{
    public Guid TenantId { get; set; }
    
    public string TenantName { get; set; }
    
    public string EditionName { get; set; }
    
    public Dictionary<string, object> FeatureLimits { get; set; }
    
    public string SubscriptionStatus { get; set; }
    
    public DateTime? SubscriptionEndDate { get; set; }
    
    public DateTime? NextBillingDate { get; set; }
    
    public int DaysRemaining { get; set; }
    
    public decimal CurrentPlanPrice { get; set; }
    
    public string BillingPeriod { get; set; }
    
    public int TotalUsers { get; set; }
    
    public int PendingInvoices { get; set; }
    
    public int OverdueInvoices { get; set; }
}
