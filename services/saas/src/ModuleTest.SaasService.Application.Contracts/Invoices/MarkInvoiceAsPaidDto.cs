using System.ComponentModel.DataAnnotations;

namespace ModuleTest.SaasService.Invoices;

/// <summary>
/// Data transfer object for marking an invoice as paid
/// </summary>
public class MarkInvoiceAsPaidDto
{
    /// <summary>
    /// Payment method used (e.g., CreditCard, PayPal, BankTransfer)
    /// </summary>
    /// <example>CreditCard</example>
    [Required]
    [StringLength(128, MinimumLength = 2)]
    public string PaymentMethod { get; set; }
    
    /// <summary>
    /// Payment transaction reference or ID
    /// </summary>
    /// <example>TXN-2025-12345</example>
    [StringLength(256)]
    public string PaymentReference { get; set; }
}
