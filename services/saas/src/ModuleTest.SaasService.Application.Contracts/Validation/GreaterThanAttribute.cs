using System;
using System.ComponentModel.DataAnnotations;

namespace ModuleTest.SaasService.Validation;

/// <summary>
/// Validates that a property value is greater than another property value.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class GreaterThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public GreaterThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

        if (comparisonProperty == null)
        {
            return new ValidationResult($"Unknown property: {_comparisonProperty}");
        }

        var comparisonValue = comparisonProperty.GetValue(validationContext.ObjectInstance);

        if (comparisonValue == null)
        {
            return ValidationResult.Success;
        }

        if (value is IComparable comparableValue && comparisonValue is IComparable comparableComparisonValue)
        {
            if (comparableValue.CompareTo(comparableComparisonValue) <= 0)
            {
                return new ValidationResult(
                    ErrorMessage ?? $"{validationContext.DisplayName} must be greater than {_comparisonProperty}");
            }
        }

        return ValidationResult.Success;
    }
}
