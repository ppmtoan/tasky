using System;
using System.ComponentModel.DataAnnotations;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.Validation;

namespace ModuleTest.SaasService.Subscriptions;

/// <summary>
/// Data transfer object for creating a new subscription
/// </summary>
public class CreateSubscriptionDto
{
    /// <summary>
    /// Tenant ID for the subscription (optional for host)
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid? TenantId { get; set; }
    
    /// <summary>
    /// Edition/plan ID to subscribe to
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Required]
    public Guid EditionId { get; set; }
    
    /// <summary>
    /// Billing cycle (0=Monthly, 1=Yearly)
    /// </summary>
    /// <example>1</example>
    [Required]
    public BillingPeriod BillingPeriod { get; set; }
    
    /// <summary>
    /// Subscription start date (defaults to now if not specified)
    /// </summary>
    /// <example>2025-01-15T00:00:00Z</example>
    [ValidDateRange(MinDaysFromNow = 0, MaxDaysFromNow = 365, ErrorMessage = "Start date must be within the next 365 days")]
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    /// Subscription price
    /// </summary>
    /// <example>499.99</example>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Enable automatic renewal at the end of billing period
    /// </summary>
    /// <example>true</example>
    public bool AutoRenew { get; set; } = true;
    
    /// <summary>
    /// Number of free trial days (max 90 days)
    /// </summary>
    /// <example>30</example>
    [Range(0, 90, ErrorMessage = "Trial period cannot exceed 90 days")]
    public int? TrialDays { get; set; }
}
