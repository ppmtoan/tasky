using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ModuleTest.SaasService.EntityFrameworkCore.Repositories;

public class SubscriptionRepository : EfCoreRepository<ISaasDbContext, Subscription, Guid>, ISubscriptionRepository
{
    public SubscriptionRepository(IDbContextProvider<ISaasDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }

    public async Task<Subscription> FindActiveByTenantIdAsync(Guid tenantId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(s => s.TenantId == tenantId && s.Status == SubscriptionStatus.Active)
            .OrderByDescending(s => s.CreationTime)
            .FirstOrDefaultAsync();
    }
}
