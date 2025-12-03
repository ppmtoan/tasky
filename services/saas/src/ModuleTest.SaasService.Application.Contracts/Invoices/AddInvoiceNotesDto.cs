using System.ComponentModel.DataAnnotations;

namespace ModuleTest.SaasService.Invoices;

/// <summary>
/// DTO for adding notes to an invoice
/// </summary>
public class AddInvoiceNotesDto
{
    /// <summary>
    /// Notes to add to the invoice
    /// </summary>
    [Required]
    [StringLength(2000)]
    public string Notes { get; set; }
}
