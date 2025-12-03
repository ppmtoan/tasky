using System;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ModuleTest.SaasService.Editions;

/// <summary>
/// Edition management service - temporarily allows anonymous access for testing
/// TODO: Remove [AllowAnonymous] in production
/// </summary>
[AllowAnonymous]
public interface IEditionAppService : ICrudAppService<EditionDto, Guid, PagedAndSortedResultRequestDto, CreateEditionDto, UpdateEditionDto>
{
}
