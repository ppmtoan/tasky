using System;
using ModuleTest.SaasService.ValueObjects;

namespace ModuleTest.SaasService.Events;

[Serializable]
public class InvoicePaidEvent
{
    public Guid InvoiceId { get; set; }
    
    public Guid SubscriptionId { get; set; }
    
    public Guid? TenantId { get; set; }
    
    public Money Amount { get; set; }
    
    public DateTime PaidDate { get; set; }
    
    public string PaymentMethod { get; set; }
    
    public string PaymentReference { get; set; }
    
    public InvoiceNumber InvoiceNumber { get; set; }

    public InvoicePaidEvent(
        Guid invoiceId,
        Guid subscriptionId,
        Guid? tenantId,
        Money amount,
        DateTime paidDate,
        string paymentMethod,
        string paymentReference = null,
        InvoiceNumber invoiceNumber = null)
    {
        InvoiceId = invoiceId;
        SubscriptionId = subscriptionId;
        TenantId = tenantId;
        Amount = amount;
        PaidDate = paidDate;
        PaymentMethod = paymentMethod;
        PaymentReference = paymentReference;
        InvoiceNumber = invoiceNumber;
    }
}
