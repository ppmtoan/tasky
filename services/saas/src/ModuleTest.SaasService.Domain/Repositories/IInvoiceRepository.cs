using System;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using Volo.Abp.Domain.Repositories;

namespace ModuleTest.SaasService.Repositories;

public interface IInvoiceRepository : IRepository<Invoice, Guid>
{
}
