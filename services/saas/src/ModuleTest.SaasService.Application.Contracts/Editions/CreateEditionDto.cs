using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ModuleTest.SaasService.Validation;

namespace ModuleTest.SaasService.Editions;

/// <summary>
/// Data transfer object for creating a new subscription edition
/// </summary>
public class CreateEditionDto
{
    /// <summary>
    /// Unique internal name for the edition (e.g., "basic", "professional")
    /// </summary>
    /// <example>professional</example>
    [Required]
    [StringLength(128)]
    public string Name { get; set; }
    
    /// <summary>
    /// User-friendly display name shown to customers
    /// </summary>
    /// <example>Professional Plan</example>
    [Required]
    [StringLength(256)]
    public string DisplayName { get; set; }
    
    /// <summary>
    /// Detailed description of the edition features and benefits
    /// </summary>
    /// <example>Ideal for growing businesses with advanced features and priority support</example>
    [StringLength(1024)]
    public string Description { get; set; }
    
    /// <summary>
    /// Monthly subscription price in USD
    /// </summary>
    /// <example>49.99</example>
    [Required]
    [Range(0, double.MaxValue)]
    public decimal MonthlyPrice { get; set; }
    
    /// <summary>
    /// Yearly subscription price in USD (must be greater than monthly price)
    /// </summary>
    /// <example>499.99</example>
    [Required]
    [Range(0, double.MaxValue)]
    [GreaterThan(nameof(MonthlyPrice), ErrorMessage = "Yearly price must be greater than monthly price")]
    public decimal YearlyPrice { get; set; }
    
    /// <summary>
    /// Indicates whether the edition is available for new subscriptions
    /// </summary>
    /// <example>true</example>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Sort order for displaying editions (lower numbers appear first)
    /// </summary>
    /// <example>1</example>
    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Feature limits and quotas for this edition. Required keys: MaxUsers, MaxProjects, StorageQuotaGB, APICallsPerMonth
    /// </summary>
    /// <example>{ "MaxUsers": 10, "MaxProjects": 50, "StorageQuotaGB": 100, "APICallsPerMonth": 10000 }</example>
    [Required]
    [ValidFeatureLimits(ErrorMessage = "Feature limits must contain MaxUsers, MaxProjects, StorageQuotaGB, and APICallsPerMonth with positive values")]
    public Dictionary<string, object> FeatureLimits { get; set; }
}
