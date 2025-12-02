using System;
using System.ComponentModel.DataAnnotations;
using ModuleTest.SaasService.Enums;

namespace ModuleTest.SaasService.Subscriptions;

/// <summary>
/// Data transfer object for updating an existing subscription
/// </summary>
public class UpdateSubscriptionDto
{
    /// <summary>
    /// Updated billing cycle
    /// </summary>
    /// <example>1</example>
    [Required]
    public BillingPeriod BillingPeriod { get; set; }
    
    /// <summary>
    /// Updated subscription price
    /// </summary>
    /// <example>599.99</example>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Auto-renewal preference
    /// </summary>
    /// <example>false</example>
    public bool AutoRenew { get; set; }
}
