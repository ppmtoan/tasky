using Volo.Abp.Reflection;

namespace ModuleTest.SaasService.Permissions;

public class SaasServicePermissions
{
    public const string GroupName = "SaasService";

    public static class Tenants
    {
        public const string Default = GroupName + ".Tenants";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string ManageFeatures = Default + ".ManageFeatures";
        public const string ManageConnectionStrings = Default + ".ManageConnectionStrings";
    }

    public static class Editions
    {
        public const string Default = GroupName + ".Editions";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Subscriptions
    {
        public const string Default = GroupName + ".Subscriptions";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Manage = Default + ".Manage";
    }

    public static class Invoices
    {
        public const string Default = GroupName + ".Invoices";
        public const string MarkAsPaid = Default + ".MarkAsPaid";
        public const string Cancel = Default + ".Cancel";
    }

    public static class TenantProvisioning
    {
        public const string Default = GroupName + ".TenantProvisioning";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(SaasServicePermissions));
    }
}
