using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace ModuleTest.SaasService.Invoices;

/// <summary>
/// Invoice management service
/// Note: Apply [AllowAnonymous] or [Authorize] at controller/method level
/// </summary>
public interface IInvoiceAppService : IApplicationService
{
    Task<PagedResultDto<InvoiceDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    
    Task<InvoiceDto> GetAsync(Guid id);
    
    Task<List<InvoiceDto>> GetCurrentTenantInvoicesAsync();
    
    Task<InvoiceDto> MarkAsPaidAsync(Guid id, MarkInvoiceAsPaidDto input);
    
    Task CancelAsync(Guid id);
    
    Task ProcessOverdueInvoicesAsync();
    
    /// <summary>
    /// Adds notes to an invoice
    /// </summary>
    Task<InvoiceDto> AddNotesAsync(Guid id, AddInvoiceNotesDto input);
}
