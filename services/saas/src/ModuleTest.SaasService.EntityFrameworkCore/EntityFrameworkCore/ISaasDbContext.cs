using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace ModuleTest.SaasService.EntityFrameworkCore;

[ConnectionStringName(SaasServiceDbProperties.ConnectionStringName)]
public interface ISaasDbContext : IEfCoreDbContext
{
    DbSet<Edition> Editions { get; }
    
    DbSet<Subscription> Subscriptions { get; }
    
    DbSet<Invoice> Invoices { get; }
}
