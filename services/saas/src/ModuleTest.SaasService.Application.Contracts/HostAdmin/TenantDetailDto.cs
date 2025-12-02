using System;
using System.Collections.Generic;

namespace ModuleTest.SaasService.HostAdmin;

public class TenantDetailDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime CreationTime { get; set; }
    
    public string EditionName { get; set; }
    
    public string SubscriptionStatus { get; set; }
    
    public DateTime? SubscriptionEndDate { get; set; }
    
    public int TotalUsers { get; set; }
    
    public decimal TotalRevenue { get; set; }
    
    public List<string> ConnectionStrings { get; set; }
}
