using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ModuleTest.SaasService.Validation;

namespace ModuleTest.SaasService.Editions;

public class UpdateEditionDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(256)]
    public string DisplayName { get; set; }
    
    [StringLength(1024)]
    public string Description { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal MonthlyPrice { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    [GreaterThan(nameof(MonthlyPrice), ErrorMessage = "Yearly price must be greater than monthly price")]
    public decimal YearlyPrice { get; set; }
    
    public bool IsActive { get; set; }
    
    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }
    
    [Required]
    [ValidFeatureLimits(ErrorMessage = "Feature limits must contain MaxUsers, MaxProjects, StorageQuotaGB, and APICallsPerMonth with positive values")]
    public Dictionary<string, object> FeatureLimits { get; set; }
}
