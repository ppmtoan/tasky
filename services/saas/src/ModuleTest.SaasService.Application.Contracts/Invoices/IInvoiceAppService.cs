using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace ModuleTest.SaasService.Invoices;

/// TODO: Remove [AllowAnonymous] in production and enforce proper authorization
/// This is temporarily disabled for testing purposes
[AllowAnonymous]
public interface IInvoiceAppService : IApplicationService
{
    Task<PagedResultDto<InvoiceDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    
    Task<InvoiceDto> GetAsync(Guid id);
    
    Task<List<InvoiceDto>> GetCurrentTenantInvoicesAsync();
    
    Task<InvoiceDto> MarkAsPaidAsync(Guid id, MarkInvoiceAsPaidDto input);
    
    Task CancelAsync(Guid id);
    
    Task ProcessOverdueInvoicesAsync();
}
