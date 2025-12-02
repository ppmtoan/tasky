using ModuleTest.SaasService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace ModuleTest.SaasService.Permissions;

public class SaasServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(SaasServicePermissions.GroupName, L("Permission:SaasService"));

        // Tenants
        var tenantsPermission = myGroup.AddPermission(
            SaasServicePermissions.Tenants.Default,
            L("Permission:SaasService:Tenants")
        );
        tenantsPermission.AddChild(
            SaasServicePermissions.Tenants.Create,
            L("Permission:SaasService:Tenants.Create")
        );
        tenantsPermission.AddChild(
            SaasServicePermissions.Tenants.Update,
            L("Permission:SaasService:Tenants.Update")
        );
        tenantsPermission.AddChild(
            SaasServicePermissions.Tenants.Delete,
            L("Permission:SaasService:Tenants.Delete")
        );
        tenantsPermission.AddChild(
            SaasServicePermissions.Tenants.ManageFeatures,
            L("Permission:SaasService:Tenants.ManageFeatures")
        );
        tenantsPermission.AddChild(
            SaasServicePermissions.Tenants.ManageConnectionStrings,
            L("Permission:SaasService:Tenants.ManageConnectionStrings")
        );

        // Editions
        var editionsPermission = myGroup.AddPermission(
            SaasServicePermissions.Editions.Default,
            L("Permission:SaasService:Editions")
        );
        editionsPermission.AddChild(
            SaasServicePermissions.Editions.Create,
            L("Permission:SaasService:Editions.Create")
        );
        editionsPermission.AddChild(
            SaasServicePermissions.Editions.Update,
            L("Permission:SaasService:Editions.Update")
        );
        editionsPermission.AddChild(
            SaasServicePermissions.Editions.Delete,
            L("Permission:SaasService:Editions.Delete")
        );

        // Subscriptions
        var subscriptionsPermission = myGroup.AddPermission(
            SaasServicePermissions.Subscriptions.Default,
            L("Permission:SaasService:Subscriptions")
        );
        subscriptionsPermission.AddChild(
            SaasServicePermissions.Subscriptions.Create,
            L("Permission:SaasService:Subscriptions.Create")
        );
        subscriptionsPermission.AddChild(
            SaasServicePermissions.Subscriptions.Update,
            L("Permission:SaasService:Subscriptions.Update")
        );
        subscriptionsPermission.AddChild(
            SaasServicePermissions.Subscriptions.Delete,
            L("Permission:SaasService:Subscriptions.Delete")
        );
        subscriptionsPermission.AddChild(
            SaasServicePermissions.Subscriptions.Manage,
            L("Permission:SaasService:Subscriptions.Manage")
        );

        // Invoices
        var invoicesPermission = myGroup.AddPermission(
            SaasServicePermissions.Invoices.Default,
            L("Permission:SaasService:Invoices")
        );
        invoicesPermission.AddChild(
            SaasServicePermissions.Invoices.MarkAsPaid,
            L("Permission:SaasService:Invoices.MarkAsPaid")
        );
        invoicesPermission.AddChild(
            SaasServicePermissions.Invoices.Cancel,
            L("Permission:SaasService:Invoices.Cancel")
        );

        // Tenant Provisioning (public)
        myGroup.AddPermission(
            SaasServicePermissions.TenantProvisioning.Default,
            L("Permission:SaasService:TenantProvisioning")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SaasServiceResource>(name);
    }
}
