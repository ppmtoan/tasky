using System;
using System.ComponentModel.DataAnnotations;

namespace ModuleTest.SaasService.Validation;

/// <summary>
/// Validates that a date is within a valid range relative to another date or current date.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ValidDateRangeAttribute : ValidationAttribute
{
    public int MinDaysFromNow { get; set; } = int.MinValue;
    public int MaxDaysFromNow { get; set; } = int.MaxValue;
    public string ComparisonProperty { get; set; }
    public bool MustBeAfter { get; set; }
    public bool MustBeBefore { get; set; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value is not DateTime dateValue)
        {
            return new ValidationResult("Value must be a valid date");
        }

        // Validate against current date
        if (MinDaysFromNow != int.MinValue)
        {
            var minDate = DateTime.UtcNow.AddDays(MinDaysFromNow);
            if (dateValue < minDate)
            {
                return new ValidationResult($"{validationContext.DisplayName} must be at least {MinDaysFromNow} days from now");
            }
        }

        if (MaxDaysFromNow != int.MaxValue)
        {
            var maxDate = DateTime.UtcNow.AddDays(MaxDaysFromNow);
            if (dateValue > maxDate)
            {
                return new ValidationResult($"{validationContext.DisplayName} must be within {MaxDaysFromNow} days from now");
            }
        }

        // Validate against another property
        if (!string.IsNullOrEmpty(ComparisonProperty))
        {
            var comparisonProperty = validationContext.ObjectType.GetProperty(ComparisonProperty);
            if (comparisonProperty == null)
            {
                return new ValidationResult($"Unknown property: {ComparisonProperty}");
            }

            var comparisonValue = comparisonProperty.GetValue(validationContext.ObjectInstance);
            if (comparisonValue is DateTime comparisonDate)
            {
                if (MustBeAfter && dateValue <= comparisonDate)
                {
                    return new ValidationResult($"{validationContext.DisplayName} must be after {ComparisonProperty}");
                }

                if (MustBeBefore && dateValue >= comparisonDate)
                {
                    return new ValidationResult($"{validationContext.DisplayName} must be before {ComparisonProperty}");
                }
            }
        }

        return ValidationResult.Success;
    }
}
