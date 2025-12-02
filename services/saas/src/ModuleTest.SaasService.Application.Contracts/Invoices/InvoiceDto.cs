using System;
using ModuleTest.SaasService.Enums;

namespace ModuleTest.SaasService.Invoices;

public class InvoiceDto
{
    public Guid Id { get; set; }
    
    public Guid? TenantId { get; set; }
    
    public string TenantName { get; set; }
    
    public Guid SubscriptionId { get; set; }
    
    public string InvoiceNumber { get; set; }
    
    public DateTime InvoiceDate { get; set; }
    
    public DateTime DueDate { get; set; }
    
    public decimal Amount { get; set; }
    
    public InvoiceStatus Status { get; set; }
    
    public DateTime? PaidDate { get; set; }
    
    public string PaymentMethod { get; set; }
    
    public string PaymentReference { get; set; }
    
    public BillingPeriod BillingPeriod { get; set; }
    
    public DateTime PeriodStart { get; set; }
    
    public DateTime PeriodEnd { get; set; }
    
    public string Notes { get; set; }
    
    public bool IsOverdue { get; set; }
}
