using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Permissions;
using ModuleTest.SaasService.Repositories;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
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

    public virtual async Task ActivateAsync(Guid id)
    {
        var edition = await GetEntityByIdAsync(id);
        
        if (edition.IsActive)
        {
            throw new BusinessException(SaasServiceErrorCodes.EditionAlreadyActive)
                .WithData("EditionId", id);
        }
        
        edition.Activate();
        
        await Repository.UpdateAsync(edition);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public virtual async Task DeactivateAsync(Guid id)
    {
        var edition = await GetEntityByIdAsync(id);
        
        if (!edition.IsActive)
        {
            throw new BusinessException(SaasServiceErrorCodes.EditionAlreadyInactive)
                .WithData("EditionId", id);
        }
        
        edition.Deactivate();
        
        await Repository.UpdateAsync(edition);
        await CurrentUnitOfWork.SaveChangesAsync();
    }

    public virtual async Task UpdateDisplayOrderAsync(Guid id, int displayOrder)
    {
        if (displayOrder < 0)
        {
            throw new BusinessException(SaasServiceErrorCodes.InvalidDisplayOrder)
                .WithData("DisplayOrder", displayOrder);
        }
        
        var edition = await GetEntityByIdAsync(id);
        edition.UpdateDisplayOrder(displayOrder);
        
        await Repository.UpdateAsync(edition);
        await CurrentUnitOfWork.SaveChangesAsync();
    }
}
