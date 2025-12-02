using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ModuleTest.SaasService.DomainServices;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Enums;
using ModuleTest.SaasService.Permissions;
using ModuleTest.SaasService.Repositories;
using ModuleTest.SaasService.ValueObjects;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using Volo.Saas.Tenants;

namespace ModuleTest.SaasService.TenantProvisioning;

public class TenantProvisioningAppService : ApplicationService, ITenantProvisioningAppService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly TenantProvisioningManager _tenantProvisioningManager;

    public TenantProvisioningAppService(
        ITenantRepository tenantRepository,
        ISubscriptionRepository subscriptionRepository,
        IInvoiceRepository invoiceRepository,
        TenantProvisioningManager tenantProvisioningManager)
    {
        _tenantRepository = tenantRepository;
        _subscriptionRepository = subscriptionRepository;
        _invoiceRepository = invoiceRepository;
        _tenantProvisioningManager = tenantProvisioningManager;
    }

    [UnitOfWork]
    public virtual async Task<TenantProvisioningResultDto> ProvisionTenantAsync(TenantProvisioningRequestDto input)
    {
        await CheckPolicyAsync(SaasServicePermissions.TenantProvisioning.Default);

        try
        {
            // Delegate complex business logic to domain service
            var result = await _tenantProvisioningManager.ProvisionTenantAsync(
                input.TenantName,
                input.EditionId,
                input.BillingPeriod,
                input.AdminEmail,
                input.AdminUserName,
                input.AdminPassword,
                input.TrialDays
            );

            // Persist all entities
            await _tenantRepository.InsertAsync(result.Tenant);
            await _subscriptionRepository.InsertAsync(result.Subscription);
            
            if (result.InitialInvoice != null)
            {
                await _invoiceRepository.InsertAsync(result.InitialInvoice);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            Logger.LogInformation($"Successfully provisioned tenant: {input.TenantName}");

            return new TenantProvisioningResultDto
            {
                TenantId = result.Tenant.Id,
                TenantName = result.Tenant.Name,
                SubscriptionId = result.Subscription.Id,
                AdminUserId = result.AdminUserId,
                AdminEmail = input.AdminEmail,
                Success = true,
                Message = "Tenant provisioned successfully"
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to provision tenant: {input.TenantName}");
            throw;
        }
    }
}
