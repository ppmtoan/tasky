using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModuleTest.SaasService.Validation;

/// <summary>
/// Validates that feature limits dictionary has valid structure and values.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ValidFeatureLimitsAttribute : ValidationAttribute
{
    private static readonly HashSet<string> RequiredFeatures = new()
    {
        "MaxUsers",
        "MaxProjects",
        "StorageQuotaGB",
        "APICallsPerMonth"
    };

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Feature limits are required");
        }

        if (value is not Dictionary<string, object> featureLimits)
        {
            return new ValidationResult("Feature limits must be a valid dictionary");
        }

        // Check for required features
        foreach (var requiredFeature in RequiredFeatures)
        {
            if (!featureLimits.ContainsKey(requiredFeature))
            {
                return new ValidationResult($"Feature limit '{requiredFeature}' is required");
            }
        }

        // Validate numeric values
        foreach (var kvp in featureLimits)
        {
            if (kvp.Key.StartsWith("Max") || kvp.Key.EndsWith("GB") || kvp.Key.EndsWith("PerMonth"))
            {
                if (!IsValidPositiveInteger(kvp.Value))
                {
                    return new ValidationResult($"Feature limit '{kvp.Key}' must be a positive integer");
                }
            }
        }

        return ValidationResult.Success;
    }

    private bool IsValidPositiveInteger(object value)
    {
        if (value == null)
        {
            return false;
        }

        return value switch
        {
            int intValue => intValue > 0,
            long longValue => longValue > 0,
            string strValue => int.TryParse(strValue, out var result) && result > 0,
            _ => false
        };
    }
}
