using System;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ModuleTest.SaasService.EntityFrameworkCore.Repositories;

public class InvoiceRepository : EfCoreRepository<ISaasDbContext, Invoice, Guid>, IInvoiceRepository
{
    public InvoiceRepository(IDbContextProvider<ISaasDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }
}
