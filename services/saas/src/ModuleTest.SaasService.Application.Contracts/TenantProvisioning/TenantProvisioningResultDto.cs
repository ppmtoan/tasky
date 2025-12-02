using System;

namespace ModuleTest.SaasService.TenantProvisioning;

public class TenantProvisioningResultDto
{
    public Guid TenantId { get; set; }
    
    public string TenantName { get; set; }
    
    public Guid SubscriptionId { get; set; }
    
    public Guid AdminUserId { get; set; }
    
    public string AdminEmail { get; set; }
    
    public bool Success { get; set; }
    
    public string Message { get; set; }
}
