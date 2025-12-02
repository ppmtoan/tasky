using System;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.Events;
using ModuleTest.SaasService.ValueObjects;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ModuleTest.SaasService.Aggregates.BillingAggregate;

/// <summary>
/// Represents an invoice for a subscription billing period
/// Aggregate Root - controls all changes through methods
/// </summary>
public class Invoice : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    
    public Guid SubscriptionId { get; private set; }
    
    public Subscription Subscription { get; private set; }
    
    public InvoiceNumber InvoiceNumber { get; private set; }
    
    public DateTime InvoiceDate { get; private set; }
    
    public DateTime DueDate { get; private set; }
    
    public Money Amount { get; private set; }
    
    public InvoiceStatus Status { get; private set; }
    
    public DateTime? PaidDate { get; private set; }
    
    public string PaymentMethod { get; private set; }
    
    public string PaymentReference { get; private set; }
    
    public BillingPeriod BillingPeriod { get; private set; }
    
    public DateRange BillingPeriodRange { get; private set; }
    
    public string Notes { get; private set; }

    protected Invoice()
    {
    }

    public Invoice(
        Guid id,
        Guid? tenantId,
        Guid subscriptionId,
        InvoiceNumber invoiceNumber,
        DateTime invoiceDate,
        DateTime dueDate,
        Money amount,
        BillingPeriod billingPeriod,
        DateRange billingPeriodRange) : base(id)
    {
        if (subscriptionId == Guid.Empty)
            throw new ArgumentException("Subscription ID cannot be empty", nameof(subscriptionId));
        
        if (invoiceNumber == null)
            throw new ArgumentNullException(nameof(invoiceNumber));
        
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));
        
        if (dueDate < invoiceDate)
            throw new ArgumentException("Due date cannot be before invoice date", nameof(dueDate));
        
        if (billingPeriodRange == null)
            throw new ArgumentNullException(nameof(billingPeriodRange));
        
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        InvoiceNumber = invoiceNumber;
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        Amount = amount;
        Status = InvoiceStatus.Pending;
        BillingPeriod = billingPeriod;
        BillingPeriodRange = billingPeriodRange;
    }

    public void MarkAsPaid(string paymentMethod = null, string paymentReference = null)
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Invoice is already paid");
        
        if (Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException("Cannot pay a cancelled invoice");
        
        Status = InvoiceStatus.Paid;
        PaidDate = DateTime.UtcNow;
        PaymentMethod = paymentMethod ?? "Manual";
        PaymentReference = paymentReference;
        
        AddDistributedEvent(new InvoicePaidEvent(
            invoiceId: Id,
            subscriptionId: SubscriptionId,
            tenantId: TenantId,
            amount: Amount,
            paidDate: PaidDate.Value,
            paymentMethod: PaymentMethod,
            paymentReference: PaymentReference,
            invoiceNumber: InvoiceNumber
        ));
    }

    public void MarkAsOverdue()
    {
        if (Status == InvoiceStatus.Pending && DateTime.UtcNow > DueDate)
        {
            Status = InvoiceStatus.Overdue;
        }
    }

    public void Cancel()
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot cancel a paid invoice");
        
        if (Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException("Invoice is already cancelled");
        
        Status = InvoiceStatus.Cancelled;
    }

    public void AddNotes(string notes)
    {
        if (!string.IsNullOrWhiteSpace(notes))
        {
            Notes = string.IsNullOrWhiteSpace(Notes) 
                ? notes 
                : $"{Notes}\n{notes}";
        }
    }

    public bool CanBePaid()
    {
        return Status == InvoiceStatus.Pending || Status == InvoiceStatus.Overdue;
    }

    public bool CanBeCancelled()
    {
        return Status == InvoiceStatus.Pending;
    }

    public bool IsOverdue()
    {
        return Status == InvoiceStatus.Pending && DateTime.UtcNow > DueDate;
    }

    public int DaysOverdue()
    {
        if (!IsOverdue())
        {
            return 0;
        }

        return (DateTime.UtcNow - DueDate).Days;
    }

    public int DaysUntilDue()
    {
        if (Status != InvoiceStatus.Pending)
        {
            return 0;
        }

        var days = (DueDate - DateTime.UtcNow).Days;
        return days > 0 ? days : 0;
    }
}
