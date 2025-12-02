using System;
using System.ComponentModel.DataAnnotations;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.Validation;

namespace ModuleTest.SaasService.TenantProvisioning;

public class TenantProvisioningRequestDto
{
    [Required]
    [StringLength(64, MinimumLength = 3)]
    public string TenantName { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string AdminEmail { get; set; }
    
    [Required]
    [StringLength(128, MinimumLength = 6)]
    public string AdminPassword { get; set; }
    
    [StringLength(128, MinimumLength = 3)]
    public string AdminUserName { get; set; }
    
    [Required]
    public Guid EditionId { get; set; }
    
    [Required]
    public BillingPeriod BillingPeriod { get; set; }
    
    [Range(0, 90, ErrorMessage = "Trial period cannot exceed 90 days")]
    public int? TrialDays { get; set; }
}
