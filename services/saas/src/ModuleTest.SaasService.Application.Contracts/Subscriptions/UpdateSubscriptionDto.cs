using System;
using System.ComponentModel.DataAnnotations;
using ModuleTest.SaasService.Enums;

namespace ModuleTest.SaasService.Subscriptions;

public class UpdateSubscriptionDto
{
    [Required]
    public BillingPeriod BillingPeriod { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    public bool AutoRenew { get; set; }
}
