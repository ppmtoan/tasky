using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleTest.SaasService.Invoices;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace ModuleTest.SaasService.Controllers.Invoices;

[RemoteService(Name = SaasServiceRemoteServiceConsts.RemoteServiceName)]
[Area(SaasServiceRemoteServiceConsts.ModuleName)]
[Route("api/saas-service/invoices")] // Changed to avoid conflict with ABP's built-in SaaS module
[AllowAnonymous] // For testing purposes - remove in production
public class InvoiceController : AbpControllerBase, IInvoiceAppService
{
    private readonly IInvoiceAppService _invoiceAppService;

    public InvoiceController(IInvoiceAppService invoiceAppService)
    {
        _invoiceAppService = invoiceAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<InvoiceDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        return _invoiceAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<InvoiceDto> GetAsync(Guid id)
    {
        return _invoiceAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("current-tenant")]
    public virtual Task<List<InvoiceDto>> GetCurrentTenantInvoicesAsync()
    {
        return _invoiceAppService.GetCurrentTenantInvoicesAsync();
    }

    [HttpPost]
    [Route("{id}/mark-as-paid")]
    public virtual Task<InvoiceDto> MarkAsPaidAsync(Guid id, MarkInvoiceAsPaidDto input)
    {
        return _invoiceAppService.MarkAsPaidAsync(id, input);
    }

    [HttpPost]
    [Route("{id}/cancel")]
    public virtual Task CancelAsync(Guid id)
    {
        return _invoiceAppService.CancelAsync(id);
    }

    [HttpPost]
    [Route("process-overdue")]
    public virtual Task ProcessOverdueInvoicesAsync()
    {
        return _invoiceAppService.ProcessOverdueInvoicesAsync();
    }
}
