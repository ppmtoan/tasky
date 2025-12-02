using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ModuleTest.SaasService.Invoices;

public interface IInvoiceAppService : IApplicationService
{
    Task<PagedResultDto<InvoiceDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    
    Task<InvoiceDto> GetAsync(Guid id);
    
    Task<List<InvoiceDto>> GetCurrentTenantInvoicesAsync();
    
    Task<InvoiceDto> MarkAsPaidAsync(Guid id, MarkInvoiceAsPaidDto input);
    
    Task CancelAsync(Guid id);
    
    Task ProcessOverdueInvoicesAsync();
}
