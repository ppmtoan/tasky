using System;

namespace ModuleTest.SaasService.TenantProvisioning;

public class TenantProvisioningResultDto
{
    public Guid TenantId { get; set; }
    
    public string TenantName { get; set; }
    
    public Guid SubscriptionId { get; set; }
    
    /// <summary>
    /// Admin email (AdminUserId will be created by Identity service via event)
    /// </summary>
    public string AdminEmail { get; set; }
    
    public bool Success { get; set; }
    
    public string Message { get; set; }
}
