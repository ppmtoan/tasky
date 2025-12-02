using System.ComponentModel.DataAnnotations;

namespace ModuleTest.SaasService.Invoices;

public class MarkInvoiceAsPaidDto
{
    [Required]
    [StringLength(128, MinimumLength = 2)]
    public string PaymentMethod { get; set; }
    
    [StringLength(256)]
    public string PaymentReference { get; set; }
}
