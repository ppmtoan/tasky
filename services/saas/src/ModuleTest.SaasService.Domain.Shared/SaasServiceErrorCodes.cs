namespace ModuleTest.SaasService;

public static class SaasServiceErrorCodes
{
    // Edition errors
    public const string EditionNotFound = "SaaS:001";
    public const string EditionNotActive = "SaaS:001A";
    
    // Subscription errors
    public const string SubscriptionNotFound = "SaaS:002";
    public const string SubscriptionExpired = "SaaS:007";
    public const string TenantAlreadyHasActiveSubscription = "SaaS:002A";
    public const string InvalidSubscriptionPrice = "SaaS:002B";
    public const string CannotRenewSubscription = "SaaS:002C";
    public const string CannotUpgradeInactiveSubscription = "SaaS:002D";
    public const string CannotDowngradeInactiveSubscription = "SaaS:002E";
    public const string CannotSuspendInactiveSubscription = "SaaS:002F";
    public const string CannotInvoiceInactiveSubscription = "SaaS:002G";
    public const string InvalidSubscriptionStatusTransition = "SaaS:002H";
    
    // Invoice errors
    public const string InvoiceNotFound = "SaaS:003";
    public const string InvoiceAlreadyPaid = "SaaS:008";
    public const string CannotPayInvoice = "SaaS:003A";
    public const string InvalidInvoiceAmount = "SaaS:003B";
    public const string InvalidCreditAmount = "SaaS:003C";
    
    // Tenant errors
    public const string TenantNotAvailable = "SaaS:004";
    public const string TenantAlreadyExists = "SaaS:005";
    public const string TenantMismatch = "SaaS:004A";
    
    // User errors
    public const string UserEmailAlreadyExists = "SaaS:009";
    public const string UserCreationFailed = "SaaS:010";
    
    // Billing errors
    public const string InvalidBillingPeriod = "SaaS:006";
}
