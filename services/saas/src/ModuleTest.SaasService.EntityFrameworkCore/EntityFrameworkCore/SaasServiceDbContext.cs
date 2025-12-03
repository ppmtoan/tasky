using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Saas.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;

namespace ModuleTest.SaasService.EntityFrameworkCore;

[ConnectionStringName(SaasServiceDbProperties.ConnectionStringName)]
public class SaasServiceDbContext(DbContextOptions<SaasServiceDbContext> options)
    : AbpDbContext<SaasServiceDbContext>(options),
        ITenantManagementDbContext,
        ISaasDbContext
{
    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    public DbSet<Edition> Editions { get; set; }

    public DbSet<Subscription> Subscriptions { get; set; }

    public DbSet<Invoice> Invoices { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureSaaS();
        builder.ConfigureTenantManagement();
    }
}
