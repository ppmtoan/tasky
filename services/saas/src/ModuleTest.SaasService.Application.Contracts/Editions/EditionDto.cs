using System;
using System.Collections.Generic;

namespace ModuleTest.SaasService.Editions;

public class EditionDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string DisplayName { get; set; }
    
    public string Description { get; set; }
    
    public decimal MonthlyPrice { get; set; }
    
    public decimal YearlyPrice { get; set; }
    
    public bool IsActive { get; set; }
    
    public int DisplayOrder { get; set; }
    
    public Dictionary<string, object> FeatureLimits { get; set; }
}
