using System;
using System.ComponentModel.DataAnnotations;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.Validation;

namespace ModuleTest.SaasService.TenantProvisioning;

/// <summary>
/// Data transfer object for provisioning a new tenant with subscription
/// </summary>
public class TenantProvisioningRequestDto
{
    /// <summary>
    /// Unique tenant name (used for subdomain or URL identification)
    /// </summary>
    /// <example>acme-corp</example>
    [Required]
    [StringLength(64, MinimumLength = 3)]
    public string TenantName { get; set; }
    
    /// <summary>
    /// Administrator email address
    /// </summary>
    /// <example>admin@acme-corp.com</example>
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string AdminEmail { get; set; }
    
    /// <summary>
    /// Administrator password (min 6 characters)
    /// </summary>
    /// <example>P@ssw0rd123!</example>
    [Required]
    [StringLength(128, MinimumLength = 6)]
    public string AdminPassword { get; set; }
    
    /// <summary>
    /// Administrator username (defaults to email if not provided)
    /// </summary>
    /// <example>admin</example>
    [StringLength(128, MinimumLength = 3)]
    public string AdminUserName { get; set; }
    
    /// <summary>
    /// Edition/plan ID to assign to the new tenant
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Required]
    public Guid EditionId { get; set; }
    
    /// <summary>
    /// Billing cycle for the subscription
    /// </summary>
    /// <example>1</example>
    [Required]
    public BillingPeriod BillingPeriod { get; set; }
    
    /// <summary>
    /// Optional trial period in days (max 90)
    /// </summary>
    /// <example>14</example>
    [Range(0, 90, ErrorMessage = "Trial period cannot exceed 90 days")]
    public int? TrialDays { get; set; }
}
