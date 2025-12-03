using AutoMapper;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Editions;
using ModuleTest.SaasService.Invoices;
using ModuleTest.SaasService.Subscriptions;
using ModuleTest.SaasService.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModuleTest.SaasService.Application;

public class SaasServiceApplicationAutoMapperProfile : Profile
{
    public SaasServiceApplicationAutoMapperProfile()
    {
        // Edition mappings
        CreateMap<Edition, EditionDto>()
            .ForMember(dest => dest.MonthlyPrice, opt => opt.MapFrom(src => src.MonthlyPrice.Amount))
            .ForMember(dest => dest.YearlyPrice, opt => opt.MapFrom(src => src.YearlyPrice.Amount))
            .ForMember(dest => dest.FeatureLimits, opt => opt.MapFrom(src => MapFeatureLimitsToDictionary(src.FeatureLimits)));

        CreateMap<CreateEditionDto, Edition>()
            .ConvertUsing((src, dest, context) => new Edition(
                id: Guid.NewGuid(),
                name: src.Name,
                displayName: src.DisplayName,
                description: src.Description,
                monthlyPrice: new Money(src.MonthlyPrice, "USD"),
                yearlyPrice: new Money(src.YearlyPrice, "USD"),
                featureLimits: MapDictionaryToFeatureLimits(src.FeatureLimits),
                isActive: src.IsActive,
                displayOrder: src.DisplayOrder
            ));

        CreateMap<UpdateEditionDto, Edition>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.UpdateDisplayName(src.DisplayName);
                dest.UpdateDescription(src.Description);
                dest.UpdatePricing(
                    new Money(src.MonthlyPrice, "USD"),
                    new Money(src.YearlyPrice, "USD")
                );
                dest.UpdateFeatureLimits(MapDictionaryToFeatureLimits(src.FeatureLimits));
                dest.UpdateDisplayOrder(src.DisplayOrder);
                
                if (src.IsActive)
                    dest.Activate();
                else
                    dest.Deactivate();
            });

        // Subscription mappings
        CreateMap<Subscription, SubscriptionDto>()
            .ForMember(dest => dest.TenantName, opt => opt.Ignore()) // Populated separately
            .ForMember(dest => dest.EditionName, opt => opt.Ignore()) // Populated separately
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.SubscriptionPeriod.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.SubscriptionPeriod.EndDate))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.DaysRemaining, opt => opt.MapFrom(src => src.DaysRemaining()));

        CreateMap<CreateSubscriptionDto, Subscription>()
            .ConvertUsing((src, dest, context) => new Subscription(
                id: Guid.NewGuid(),
                tenantId: src.TenantId,
                editionId: src.EditionId,
                billingPeriod: src.BillingPeriod,
                startDate: src.StartDate ?? DateTime.UtcNow,
                price: new Money(src.Price, "USD"),
                autoRenew: src.AutoRenew,
                trialDays: src.TrialDays
            ));

        CreateMap<UpdateSubscriptionDto, Subscription>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.UpdateBillingPeriod(src.BillingPeriod, new Money(src.Price, "USD"));
                
                if (src.AutoRenew)
                    dest.EnableAutoRenew();
                else
                    dest.DisableAutoRenew();
            });

        // Invoice mappings
        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.TenantName, opt => opt.Ignore()) // Populated separately
            .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.InvoiceNumber.Value))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
            .ForMember(dest => dest.PeriodStart, opt => opt.MapFrom(src => src.BillingPeriodRange.StartDate))
            .ForMember(dest => dest.PeriodEnd, opt => opt.MapFrom(src => src.BillingPeriodRange.EndDate))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue()));
    }

    private static Dictionary<string, object> MapFeatureLimitsToDictionary(FeatureLimits featureLimits)
    {
        return new Dictionary<string, object>
        {
            { "MaxUsers", featureLimits.MaxUsers },
            { "MaxProjects", featureLimits.MaxProjects },
            { "StorageQuotaGB", featureLimits.StorageQuotaGB },
            { "APICallsPerMonth", featureLimits.APICallsPerMonth },
            { "EnableAdvancedReports", featureLimits.EnableAdvancedReports },
            { "EnablePrioritySupport", featureLimits.EnablePrioritySupport },
            { "EnableCustomBranding", featureLimits.EnableCustomBranding }
        };
    }

    private static FeatureLimits MapDictionaryToFeatureLimits(Dictionary<string, object> dict)
    {
        return new FeatureLimits(
            maxUsers: GetIntValue(dict, "MaxUsers", 5),
            maxProjects: GetIntValue(dict, "MaxProjects", 3),
            storageQuotaGB: GetIntValue(dict, "StorageQuotaGB", 10),
            apiCallsPerMonth: GetIntValue(dict, "APICallsPerMonth", 1000),
            enableAdvancedReports: GetBoolValue(dict, "EnableAdvancedReports", false),
            enablePrioritySupport: GetBoolValue(dict, "EnablePrioritySupport", false),
            enableCustomBranding: GetBoolValue(dict, "EnableCustomBranding", false)
        );
    }

    private static int GetIntValue(Dictionary<string, object> dict, string key, int defaultValue)
    {
        if (dict.TryGetValue(key, out var value))
        {
            if (value is int intValue)
                return intValue;
            if (value is long longValue)
                return (int)longValue;
            if (int.TryParse(value.ToString(), out var parsedValue))
                return parsedValue;
        }
        return defaultValue;
    }

    private static bool GetBoolValue(Dictionary<string, object> dict, string key, bool defaultValue)
    {
        if (dict.TryGetValue(key, out var value))
        {
            if (value is bool boolValue)
                return boolValue;
            if (bool.TryParse(value.ToString(), out var parsedValue))
                return parsedValue;
        }
        return defaultValue;
    }
}
