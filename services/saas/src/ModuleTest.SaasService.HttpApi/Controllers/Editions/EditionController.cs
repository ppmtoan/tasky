using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleTest.SaasService.Editions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace ModuleTest.SaasService.Controllers.Editions;

/// <summary>
/// Manages subscription editions/plans with pricing and feature limits
/// </summary>
[RemoteService(Name = SaasServiceRemoteServiceConsts.RemoteServiceName)]
[Area(SaasServiceRemoteServiceConsts.ModuleName)]
[Route("api/saas-service/editions")] // Changed from "api/saas/editions" to avoid conflict with ABP's built-in SaaS module
[AllowAnonymous] // For testing purposes - remove in production
public class EditionController : AbpControllerBase, IEditionAppService
{
    private readonly IEditionAppService _editionAppService;

    public EditionController(IEditionAppService editionAppService)
    {
        _editionAppService = editionAppService;
    }

    /// <summary>
    /// Retrieves a paginated list of all editions
    /// </summary>
    /// <param name="input">Pagination and sorting parameters</param>
    /// <returns>Paged list of editions with pricing and feature information</returns>
    /// <response code="200">Returns the list of editions</response>
    [HttpGet]
    public virtual Task<PagedResultDto<EditionDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        return _editionAppService.GetListAsync(input);
    }

    /// <summary>
    /// Retrieves a specific edition by ID
    /// </summary>
    /// <param name="id">The unique identifier of the edition</param>
    /// <returns>Edition details including pricing and features</returns>
    /// <response code="200">Returns the edition details</response>
    /// <response code="404">Edition not found</response>
    [HttpGet]
    [Route("{id}")]
    public virtual Task<EditionDto> GetAsync(Guid id)
    {
        return _editionAppService.GetAsync(id);
    }

    /// <summary>
    /// Creates a new subscription edition/plan
    /// </summary>
    /// <param name="input">Edition details including name, pricing, and feature limits</param>
    /// <returns>The newly created edition with generated ID</returns>
    /// <response code="200">Edition created successfully</response>
    /// <response code="400">Invalid input data or validation errors</response>
    [HttpPost]
    public virtual Task<EditionDto> CreateAsync(CreateEditionDto input)
    {
        return _editionAppService.CreateAsync(input);
    }

    /// <summary>
    /// Updates an existing edition's details
    /// </summary>
    /// <param name="id">The unique identifier of the edition to update</param>
    /// <param name="input">Updated edition details</param>
    /// <returns>The updated edition</returns>
    /// <response code="200">Edition updated successfully</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="404">Edition not found</response>
    [HttpPut]
    [Route("{id}")]
    public virtual Task<EditionDto> UpdateAsync(Guid id, UpdateEditionDto input)
    {
        return _editionAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// Deletes an edition (only if no active subscriptions exist)
    /// </summary>
    /// <param name="id">The unique identifier of the edition to delete</param>
    /// <response code="204">Edition deleted successfully</response>
    /// <response code="400">Cannot delete edition with active subscriptions</response>
    /// <response code="404">Edition not found</response>
    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _editionAppService.DeleteAsync(id);
    }

    /// <summary>
    /// Activates an edition making it available for new subscriptions
    /// </summary>
    /// <param name="id">The unique identifier of the edition</param>
    /// <response code="200">Edition activated successfully</response>
    /// <response code="404">Edition not found</response>
    [HttpPost]
    [Route("{id}/activate")]
    public virtual Task ActivateAsync(Guid id)
    {
        return _editionAppService.ActivateAsync(id);
    }

    /// <summary>
    /// Deactivates an edition preventing new subscriptions
    /// </summary>
    /// <param name="id">The unique identifier of the edition</param>
    /// <response code="200">Edition deactivated successfully</response>
    /// <response code="404">Edition not found</response>
    [HttpPost]
    [Route("{id}/deactivate")]
    public virtual Task DeactivateAsync(Guid id)
    {
        return _editionAppService.DeactivateAsync(id);
    }

    /// <summary>
    /// Updates the display order for edition sorting
    /// </summary>
    /// <param name="id">The unique identifier of the edition</param>
    /// <param name="displayOrder">The new display order value</param>
    /// <response code="200">Display order updated successfully</response>
    /// <response code="404">Edition not found</response>
    [HttpPut]
    [Route("{id}/display-order/{displayOrder}")]
    public virtual Task UpdateDisplayOrderAsync(Guid id, int displayOrder)
    {
        return _editionAppService.UpdateDisplayOrderAsync(id, displayOrder);
    }
}
