using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ModuleTest.SaasService.Editions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace ModuleTest.SaasService.Controllers.Editions;

[RemoteService(Name = SaasServiceRemoteServiceConsts.RemoteServiceName)]
[Area(SaasServiceRemoteServiceConsts.ModuleName)]
[Route("api/saas-service/editions")]
public class EditionController : AbpControllerBase, IEditionAppService
{
    private readonly IEditionAppService _editionAppService;

    public EditionController(IEditionAppService editionAppService)
    {
        _editionAppService = editionAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<EditionDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        return _editionAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<EditionDto> GetAsync(Guid id)
    {
        return _editionAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<EditionDto> CreateAsync(CreateEditionDto input)
    {
        return _editionAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<EditionDto> UpdateAsync(Guid id, UpdateEditionDto input)
    {
        return _editionAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _editionAppService.DeleteAsync(id);
    }
}
