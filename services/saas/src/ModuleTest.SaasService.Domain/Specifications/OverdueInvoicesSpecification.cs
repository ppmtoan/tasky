using System;
using System.Linq.Expressions;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Enums;
using Volo.Abp.Specifications;

namespace ModuleTest.SaasService.Specifications;

/// <summary>
/// Specification for finding overdue invoices.
/// An invoice is overdue if its status is Pending and the due date has passed.
/// </summary>
public class OverdueInvoicesSpecification : Specification<Invoice>
{
    private readonly DateTime? _asOfDate;
    
    public OverdueInvoicesSpecification(DateTime? asOfDate = null)
    {
        _asOfDate = asOfDate;
    }
    
    public override Expression<Func<Invoice, bool>> ToExpression()
    {
        var checkDate = _asOfDate ?? DateTime.UtcNow;
        
        return invoice => 
            invoice.Status == InvoiceStatus.Pending &&
            invoice.DueDate < checkDate;
    }
}

/// <summary>
/// Specification for finding pending invoices for a specific tenant.
/// </summary>
public class TenantPendingInvoicesSpecification : Specification<Invoice>
{
    private readonly Guid _tenantId;
    
    public TenantPendingInvoicesSpecification(Guid tenantId)
    {
        _tenantId = tenantId;
    }
    
    public override Expression<Func<Invoice, bool>> ToExpression()
    {
        return invoice => 
            invoice.TenantId == _tenantId &&
            (invoice.Status == InvoiceStatus.Pending || invoice.Status == InvoiceStatus.Overdue);
    }
}

/// <summary>
/// Specification for finding invoices due within a certain number of days.
/// </summary>
public class InvoicesDueSoonSpecification : Specification<Invoice>
{
    private readonly int _daysUntilDue;
    
    public InvoicesDueSoonSpecification(int daysUntilDue)
    {
        _daysUntilDue = daysUntilDue;
    }
    
    public override Expression<Func<Invoice, bool>> ToExpression()
    {
        var now = DateTime.UtcNow;
        var dueThreshold = now.AddDays(_daysUntilDue);
        
        return invoice => 
            invoice.Status == InvoiceStatus.Pending &&
            invoice.DueDate > now &&
            invoice.DueDate <= dueThreshold;
    }
}
