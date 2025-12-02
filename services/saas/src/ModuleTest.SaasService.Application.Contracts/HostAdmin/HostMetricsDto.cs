namespace ModuleTest.SaasService.HostAdmin;

public class HostMetricsDto
{
    public int TotalTenants { get; set; }
    
    public int ActiveTenants { get; set; }
    
    public int TrialTenants { get; set; }
    
    public int ExpiredTenants { get; set; }
    
    public int SuspendedTenants { get; set; }
    
    public decimal MonthlyRecurringRevenue { get; set; }
    
    public decimal YearlyRecurringRevenue { get; set; }
    
    public int TotalSubscriptions { get; set; }
    
    public int PendingInvoices { get; set; }
    
    public int OverdueInvoices { get; set; }
    
    public decimal TotalRevenue { get; set; }
}
