using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ModuleTest.SaasService.Validation;

namespace ModuleTest.SaasService.Editions;

/// <summary>
/// Data transfer object for updating an existing edition
/// </summary>
public class UpdateEditionDto
{
    /// <summary>
    /// Unique internal name for the edition
    /// </summary>
    /// <example>professional-v2</example>
    [Required]
    [StringLength(128)]
    public string Name { get; set; }
    
    /// <summary>
    /// User-friendly display name
    /// </summary>
    /// <example>Professional Plan v2</example>
    [Required]
    [StringLength(256)]
    public string DisplayName { get; set; }
    
    /// <summary>
    /// Detailed description of the edition
    /// </summary>
    /// <example>Updated professional plan with enhanced features</example>
    [StringLength(1024)]
    public string Description { get; set; }
    
    /// <summary>
    /// Monthly subscription price in USD
    /// </summary>
    /// <example>59.99</example>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal MonthlyPrice { get; set; }
    
    /// <summary>
    /// Yearly subscription price in USD
    /// </summary>
    /// <example>599.99</example>
    [Required]
    [Range(0, double.MaxValue)]
    [GreaterThan(nameof(MonthlyPrice), ErrorMessage = "Yearly price must be greater than monthly price")]
    public decimal YearlyPrice { get; set; }
    
    /// <summary>
    /// Edition availability status
    /// </summary>
    /// <example>true</example>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Sort order for display
    /// </summary>
    /// <example>2</example>
    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Feature limits and quotas
    /// </summary>
    /// <example>{ "MaxUsers": 20, "MaxProjects": 100, "StorageQuotaGB": 200, "APICallsPerMonth": 50000 }</example>
    [Required]
    [ValidFeatureLimits(ErrorMessage = "Feature limits must contain MaxUsers, MaxProjects, StorageQuotaGB, and APICallsPerMonth with positive values")]
    public Dictionary<string, object> FeatureLimits { get; set; }
}
