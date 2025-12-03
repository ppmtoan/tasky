using System;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ModuleTest.SaasService.EntityFrameworkCore.Repositories;

public class EditionRepository : EfCoreRepository<ISaasDbContext, Edition, Guid>, IEditionRepository
{
    public EditionRepository(IDbContextProvider<ISaasDbContext> dbContextProvider) 
        : base(dbContextProvider)
    {
    }
}
