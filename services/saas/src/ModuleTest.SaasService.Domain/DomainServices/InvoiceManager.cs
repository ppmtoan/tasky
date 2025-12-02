using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.Repositories;
using ModuleTest.SaasService.Specifications;
using ModuleTest.SaasService.ValueObjects;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace ModuleTest.SaasService.DomainServices;

/// <summary>
/// Domain service responsible for invoice generation and payment processing logic.
/// Handles complex billing calculations and invoice lifecycle.
/// </summary>
public class InvoiceManager : DomainService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public InvoiceManager(
        IInvoiceRepository invoiceRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _invoiceRepository = invoiceRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    /// <summary>
    /// Generates an invoice for a subscription billing cycle.
    /// </summary>
    public async Task<Invoice> GenerateSubscriptionInvoiceAsync(
        Subscription subscription,
        int sequenceNumber)
    {
        Check.NotNull(subscription, nameof(subscription));

        // Business Rule: Can only invoice active subscriptions
        if (subscription.Status != SubscriptionStatus.Active)
        {
            throw new BusinessException(SaasServiceErrorCodes.CannotInvoiceInactiveSubscription)
                .WithData("SubscriptionId", subscription.Id)
                .WithData("Status", subscription.Status);
        }

        // Generate unique invoice number
        var invoiceNumber = InvoiceNumber.Generate(
            subscription.TenantId ?? Guid.Empty,
            Clock.Now,
            sequenceNumber
        );

        // Business Rule: Due date is 30 days from issue for standard invoices
        var dueDate = Clock.Now.AddDays(30);

        var invoice = new Invoice(
            GuidGenerator.Create(),
            subscription.TenantId ?? Guid.Empty,
            subscription.Id,
            invoiceNumber,
            Clock.Now,
            dueDate,
            subscription.Price,
            subscription.BillingPeriod,
            subscription.SubscriptionPeriod
        );

        Logger.LogInformation(
            $"Generated invoice {invoiceNumber.Value} for subscription {subscription.Id}, Amount: {subscription.Price.Amount} {subscription.Price.Currency}");

        return invoice;
    }

    /// <summary>
    /// Processes payment for an invoice with business validations.
    /// </summary>
    public void ProcessPayment(
        Invoice invoice,
        string paymentMethod,
        string paymentReference)
    {
        Check.NotNull(invoice, nameof(invoice));
        Check.NotNullOrWhiteSpace(paymentMethod, nameof(paymentMethod));

        // Business Rule: Can only pay pending or overdue invoices
        if (invoice.Status != InvoiceStatus.Pending && 
            invoice.Status != InvoiceStatus.Overdue)
        {
            throw new BusinessException()
                .WithData("InvoiceId", invoice.Id)
                .WithData("Status", invoice.Status);
        }

        invoice.MarkAsPaid(paymentMethod, paymentReference);

        Logger.LogInformation(
            $"Invoice {invoice.InvoiceNumber.Value} marked as paid. Method: {paymentMethod}, Reference: {paymentReference}");
    }

    /// <summary>
    /// Processes overdue invoices in bulk with business rules.
    /// </summary>
    public async Task<int> ProcessOverdueInvoicesAsync()
    {
        // Use specification for better reusability and testability
        var overdueSpec = new OverdueInvoicesSpecification();
        var overdueInvoices = await _invoiceRepository.GetListAsync(overdueSpec);

        foreach (var invoice in overdueInvoices)
        {
            // Business Rule: Invoice becomes overdue after due date
            invoice.MarkAsOverdue();

            Logger.LogWarning(
                $"Invoice {invoice.InvoiceNumber.Value} for tenant {invoice.TenantId} is now overdue. " +
                $"Amount: {invoice.Amount.Amount} {invoice.Amount.Currency}");
        }

        return overdueInvoices.Count;
    }

    /// <summary>
    /// Generates a prorated invoice for subscription changes.
    /// </summary>
    public Invoice GenerateProratedInvoice(
        Guid tenantId,
        Guid subscriptionId,
        Money proratedAmount,
        DateRange billingPeriod,
        int sequenceNumber,
        string description)
    {
        Check.NotNull(proratedAmount, nameof(proratedAmount));

        // Business Rule: Prorated invoices must have non-zero amounts
        if (proratedAmount.IsZero())
        {
            throw new BusinessException(SaasServiceErrorCodes.InvalidInvoiceAmount)
                .WithData("Amount", proratedAmount.Amount);
        }

        var invoiceNumber = InvoiceNumber.Generate(tenantId, Clock.Now, sequenceNumber);

        // Business Rule: Prorated invoices due immediately (7 days grace period)
        var dueDate = Clock.Now.AddDays(7);

        var invoice = new Invoice(
            GuidGenerator.Create(),
            tenantId,
            subscriptionId,
            invoiceNumber,
            Clock.Now,
            dueDate,
            proratedAmount,
            BillingPeriod.OneTime,
            billingPeriod
        );

        Logger.LogInformation(
            $"Generated prorated invoice {invoiceNumber.Value}: {description}, Amount: {proratedAmount.Amount} {proratedAmount.Currency}");

        return invoice;
    }

    /// <summary>
    /// Generates a credit note (negative invoice) for refunds or adjustments.
    /// </summary>
    public Invoice GenerateCreditNote(
        Guid tenantId,
        Guid subscriptionId,
        Money creditAmount,
        DateRange period,
        int sequenceNumber,
        string reason)
    {
        Check.NotNull(creditAmount, nameof(creditAmount));
        Check.NotNullOrWhiteSpace(reason, nameof(reason));

        // Business Rule: Credit amount must be positive (will be applied as negative)
        if (creditAmount.Amount <= 0)
        {
            throw new BusinessException(SaasServiceErrorCodes.InvalidCreditAmount)
                .WithData("Amount", creditAmount.Amount);
        }

        var invoiceNumber = InvoiceNumber.Generate(tenantId, Clock.Now, sequenceNumber);

        // Create invoice with negative amount
        var negativeAmount = new Money(-creditAmount.Amount, creditAmount.Currency);

        var creditNote = new Invoice(
            GuidGenerator.Create(),
            tenantId,
            subscriptionId,
            invoiceNumber,
            Clock.Now,
            Clock.Now, // Credit notes are immediately applicable
            negativeAmount,
            BillingPeriod.OneTime,
            period
        );

        // Auto-mark as paid since it's a credit
        creditNote.MarkAsPaid("Credit", reason);

        Logger.LogInformation(
            $"Generated credit note {invoiceNumber.Value} for {creditAmount.Amount} {creditAmount.Currency}. Reason: {reason}");

        return creditNote;
    }

    /// <summary>
    /// Calculates the next invoice sequence number for a tenant.
    /// </summary>
    public async Task<int> GetNextInvoiceSequenceNumberAsync(Guid tenantId)
    {
        var query = await _invoiceRepository.GetQueryableAsync();
        var lastInvoice = query
            .Where(i => i.TenantId == tenantId)
            .OrderByDescending(i => i.CreationTime)
            .FirstOrDefault();

        // Extract sequence from last invoice number or start at 1
        if (lastInvoice != null)
        {
            // Parse sequence from invoice number format: INV-YYYY-MM-TENANT-XXXX
            var parts = lastInvoice.InvoiceNumber.Value.Split('-');
            if (parts.Length >= 5 && int.TryParse(parts[4], out int lastSequence))
            {
                return lastSequence + 1;
            }
        }

        return 1;
    }

    /// <summary>
    /// Validates if an invoice can be cancelled based on business rules.
    /// </summary>
    public bool CanCancelInvoice(Invoice invoice)
    {
        Check.NotNull(invoice, nameof(invoice));

        // Business Rule: Can only cancel pending invoices
        if (invoice.Status != InvoiceStatus.Pending)
        {
            return false;
        }

        // Business Rule: Cannot cancel if already paid
        if (invoice.Status == InvoiceStatus.Paid)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Calculates total outstanding amount for a tenant.
    /// </summary>
    public async Task<Money> CalculateOutstandingBalanceAsync(Guid tenantId)
    {
        var query = await _invoiceRepository.GetQueryableAsync();
        var unpaidInvoices = query
            .Where(i => i.TenantId == tenantId && 
                       (i.Status == InvoiceStatus.Pending || i.Status == InvoiceStatus.Overdue))
            .ToList();

        if (!unpaidInvoices.Any())
        {
            return new Money(0);
        }

        decimal total = unpaidInvoices.Sum(i => i.Amount.Amount);
        var currency = unpaidInvoices.First().Amount.Currency;

        return new Money(total, currency);
    }
}
