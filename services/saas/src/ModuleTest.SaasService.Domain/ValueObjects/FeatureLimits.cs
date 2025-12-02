using System.Collections.Generic;
using Volo.Abp.Domain.Values;

namespace ModuleTest.SaasService.ValueObjects;

public class FeatureLimits : ValueObject
{
    public int MaxUsers { get; private set; }
    
    public int MaxProjects { get; private set; }
    
    public int StorageQuotaGB { get; private set; }
    
    public int APICallsPerMonth { get; private set; }
    
    public bool EnableAdvancedReports { get; private set; }
    
    public bool EnablePrioritySupport { get; private set; }
    
    public bool EnableCustomBranding { get; private set; }

    private FeatureLimits()
    {
        // For EF Core
    }

    public FeatureLimits(
        int maxUsers = 5,
        int maxProjects = 3,
        int storageQuotaGB = 10,
        int apiCallsPerMonth = 1000,
        bool enableAdvancedReports = false,
        bool enablePrioritySupport = false,
        bool enableCustomBranding = false)
    {
        MaxUsers = maxUsers;
        MaxProjects = maxProjects;
        StorageQuotaGB = storageQuotaGB;
        APICallsPerMonth = apiCallsPerMonth;
        EnableAdvancedReports = enableAdvancedReports;
        EnablePrioritySupport = enablePrioritySupport;
        EnableCustomBranding = enableCustomBranding;
    }

    public static FeatureLimits Free() => new FeatureLimits(
        maxUsers: 5,
        maxProjects: 3,
        storageQuotaGB: 5,
        apiCallsPerMonth: 1000,
        enableAdvancedReports: false,
        enablePrioritySupport: false,
        enableCustomBranding: false
    );

    public static FeatureLimits Basic() => new FeatureLimits(
        maxUsers: 25,
        maxProjects: 10,
        storageQuotaGB: 50,
        apiCallsPerMonth: 10000,
        enableAdvancedReports: true,
        enablePrioritySupport: false,
        enableCustomBranding: false
    );

    public static FeatureLimits Professional() => new FeatureLimits(
        maxUsers: 100,
        maxProjects: 50,
        storageQuotaGB: 200,
        apiCallsPerMonth: 100000,
        enableAdvancedReports: true,
        enablePrioritySupport: true,
        enableCustomBranding: true
    );

    public static FeatureLimits Enterprise() => new FeatureLimits(
        maxUsers: int.MaxValue,
        maxProjects: int.MaxValue,
        storageQuotaGB: int.MaxValue,
        apiCallsPerMonth: int.MaxValue,
        enableAdvancedReports: true,
        enablePrioritySupport: true,
        enableCustomBranding: true
    );

    public bool CanAddUser(int currentUsers)
    {
        return currentUsers < MaxUsers;
    }

    public bool CanAddProject(int currentProjects)
    {
        return currentProjects < MaxProjects;
    }

    public bool HasStorageAvailable(int currentStorageGB)
    {
        return currentStorageGB < StorageQuotaGB;
    }

    public bool CanMakeAPICall(int currentCalls)
    {
        return currentCalls < APICallsPerMonth;
    }

    public int RemainingUsers(int currentUsers)
    {
        return MaxUsers - currentUsers;
    }

    public int RemainingProjects(int currentProjects)
    {
        return MaxProjects - currentProjects;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return MaxUsers;
        yield return MaxProjects;
        yield return StorageQuotaGB;
        yield return APICallsPerMonth;
        yield return EnableAdvancedReports;
        yield return EnablePrioritySupport;
        yield return EnableCustomBranding;
    }
}
