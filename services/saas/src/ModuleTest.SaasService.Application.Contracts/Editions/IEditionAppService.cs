using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ModuleTest.SaasService.Editions;

/// <summary>
/// Edition management service
/// Note: Apply [AllowAnonymous] or [Authorize] at controller/method level
/// </summary>
public interface IEditionAppService : ICrudAppService<EditionDto, Guid, PagedAndSortedResultRequestDto, CreateEditionDto, UpdateEditionDto>
{
    /// <summary>
    /// Activates an edition making it available for subscription
    /// </summary>
    Task ActivateAsync(Guid id);
    
    /// <summary>
    /// Deactivates an edition preventing new subscriptions
    /// </summary>
    Task DeactivateAsync(Guid id);
    
    /// <summary>
    /// Updates the display order for sorting editions
    /// </summary>
    Task UpdateDisplayOrderAsync(Guid id, int displayOrder);
}
