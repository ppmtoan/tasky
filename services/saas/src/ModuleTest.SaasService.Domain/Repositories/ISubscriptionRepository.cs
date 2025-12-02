using System;
using System.Threading.Tasks;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using Volo.Abp.Domain.Repositories;

namespace ModuleTest.SaasService.Repositories;

public interface ISubscriptionRepository : IRepository<Subscription, Guid>
{
    Task<Subscription> FindActiveByTenantIdAsync(Guid tenantId);
}
