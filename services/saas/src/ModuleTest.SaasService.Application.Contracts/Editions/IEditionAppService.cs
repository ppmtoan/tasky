using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ModuleTest.SaasService.Editions;

public interface IEditionAppService : ICrudAppService<EditionDto, Guid, PagedAndSortedResultRequestDto, CreateEditionDto, UpdateEditionDto>
{
}
