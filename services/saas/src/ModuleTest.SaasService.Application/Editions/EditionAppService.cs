using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Permissions;
using ModuleTest.SaasService.Repositories;
using System;
using System.Text.Json;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ModuleTest.SaasService.Editions;

public class EditionAppService : CrudAppService<Edition, EditionDto, Guid, PagedAndSortedResultRequestDto, CreateEditionDto, UpdateEditionDto>, IEditionAppService
{
    public EditionAppService(IRepository<Edition, Guid> repository) : base(repository)
    {
        // TODO: Re-enable permissions in production
        // Temporarily disabled for testing - REMOVE IN PRODUCTION
        // GetPolicyName = SaasServicePermissions.Editions.Default;
        // GetListPolicyName = SaasServicePermissions.Editions.Default;
        // CreatePolicyName = SaasServicePermissions.Editions.Create;
        // UpdatePolicyName = SaasServicePermissions.Editions.Update;
        // DeletePolicyName = SaasServicePermissions.Editions.Delete;
    }
}
