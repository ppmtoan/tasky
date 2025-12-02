using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ModuleTest.Shared.Hosting.AspNetCore;

public class SwaggerSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == null)
            return;

        var properties = context.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var propertyName = GetPropertyName(property, context);
            if (propertyName == null || !schema.Properties.ContainsKey(propertyName))
                continue;

            var propertySchema = schema.Properties[propertyName];

            // Apply Required attribute
            var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
            if (requiredAttribute != null)
            {
                if (schema.Required == null)
                    schema.Required = new HashSet<string>();
                schema.Required.Add(propertyName);
            }

            // Apply Range attribute
            var rangeAttribute = property.GetCustomAttribute<RangeAttribute>();
            if (rangeAttribute != null)
            {
                if (rangeAttribute.Minimum != null)
                {
                    propertySchema.Minimum = Convert.ToDecimal(rangeAttribute.Minimum);
                }
                if (rangeAttribute.Maximum != null)
                {
                    propertySchema.Maximum = Convert.ToDecimal(rangeAttribute.Maximum);
                }
                propertySchema.Description = string.IsNullOrEmpty(propertySchema.Description)
                    ? $"Value must be between {rangeAttribute.Minimum} and {rangeAttribute.Maximum}"
                    : $"{propertySchema.Description} (Range: {rangeAttribute.Minimum} - {rangeAttribute.Maximum})";
            }

            // Apply StringLength attribute
            var stringLengthAttribute = property.GetCustomAttribute<StringLengthAttribute>();
            if (stringLengthAttribute != null)
            {
                propertySchema.MaxLength = stringLengthAttribute.MaximumLength;
                if (stringLengthAttribute.MinimumLength > 0)
                {
                    propertySchema.MinLength = stringLengthAttribute.MinimumLength;
                }
                var lengthDesc = stringLengthAttribute.MinimumLength > 0
                    ? $"Length: {stringLengthAttribute.MinimumLength}-{stringLengthAttribute.MaximumLength} characters"
                    : $"Max length: {stringLengthAttribute.MaximumLength} characters";
                propertySchema.Description = string.IsNullOrEmpty(propertySchema.Description)
                    ? lengthDesc
                    : $"{propertySchema.Description} ({lengthDesc})";
            }

            // Apply EmailAddress attribute
            var emailAttribute = property.GetCustomAttribute<EmailAddressAttribute>();
            if (emailAttribute != null)
            {
                propertySchema.Format = "email";
                propertySchema.Example = new OpenApiString("user@example.com");
            }

            // Add default values for common types
            if (propertySchema.Example == null)
            {
                propertySchema.Example = GetExampleValue(property);
            }
        }

        // Add example for the entire schema
        if (schema.Example == null)
        {
            schema.Example = GenerateSchemaExample(context.Type);
        }
    }

    private string? GetPropertyName(PropertyInfo property, SchemaFilterContext context)
    {
        var jsonPropertyName = property.Name;
        // Convert to camelCase for JSON
        return char.ToLowerInvariant(jsonPropertyName[0]) + jsonPropertyName.Substring(1);
    }

    private IOpenApiAny? GetExampleValue(PropertyInfo property)
    {
        var propertyType = property.PropertyType;
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (underlyingType == typeof(string))
        {
            if (property.Name.Contains("Email", StringComparison.OrdinalIgnoreCase))
                return new OpenApiString("user@example.com");
            if (property.Name.Contains("Name", StringComparison.OrdinalIgnoreCase))
                return new OpenApiString("Example " + property.Name);
            if (property.Name.Contains("Description", StringComparison.OrdinalIgnoreCase))
                return new OpenApiString("This is a sample description");
            if (property.Name.Contains("Password", StringComparison.OrdinalIgnoreCase))
                return new OpenApiString("P@ssw0rd123");
            return new OpenApiString("sample value");
        }

        if (underlyingType == typeof(int))
            return new OpenApiInteger(property.Name.Contains("Order") ? 1 : 100);

        if (underlyingType == typeof(decimal) || underlyingType == typeof(double))
            return new OpenApiDouble(property.Name.Contains("Price") ? 99.99 : 0);

        if (underlyingType == typeof(bool))
            return new OpenApiBoolean(true);

        if (underlyingType == typeof(DateTime))
            return new OpenApiDateTime(DateTime.UtcNow.AddDays(30));

        if (underlyingType == typeof(Guid))
            return new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6");

        return null;
    }

    private IOpenApiAny? GenerateSchemaExample(Type type)
    {
        try
        {
            var exampleObject = new OpenApiObject();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var propertyName = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
                var exampleValue = GetExampleValue(property);
                if (exampleValue != null)
                {
                    exampleObject[propertyName] = exampleValue;
                }
            }

            return exampleObject.Count > 0 ? exampleObject : null;
        }
        catch
        {
            return null;
        }
    }
}
