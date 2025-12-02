using System;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using Volo.Abp.Domain.Repositories;

namespace ModuleTest.SaasService.Repositories;

public interface IEditionRepository : IRepository<Edition, Guid>
{
}
