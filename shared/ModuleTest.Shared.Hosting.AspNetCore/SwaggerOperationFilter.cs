using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ModuleTest.Shared.Hosting.AspNetCore;

public class SwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add summary and description from XML comments if available
        if (string.IsNullOrEmpty(operation.Summary))
        {
            operation.Summary = GenerateSummary(context);
        }

        // Add response descriptions
        if (operation.Responses != null)
        {
            foreach (var response in operation.Responses)
            {
                if (string.IsNullOrEmpty(response.Value.Description))
                {
                    response.Value.Description = response.Key switch
                    {
                        "200" => "Success",
                        "201" => "Created",
                        "204" => "No Content",
                        "400" => "Bad Request - Invalid input data",
                        "401" => "Unauthorized - Authentication required",
                        "403" => "Forbidden - Insufficient permissions",
                        "404" => "Not Found - Resource does not exist",
                        "500" => "Internal Server Error",
                        _ => response.Value.Description
                    };
                }
            }
        }

        // Add request body examples for common operations
        if (operation.RequestBody?.Content != null)
        {
            foreach (var content in operation.RequestBody.Content.Values)
            {
                if (content.Schema?.Reference != null && content.Example == null)
                {
                    // Schema examples are handled by SwaggerSchemaFilter
                }
            }
        }

        // Tag operations by controller name
        if (operation.Tags == null || !operation.Tags.Any())
        {
            var controllerName = context.MethodInfo.DeclaringType?.Name.Replace("Controller", "");
            if (!string.IsNullOrEmpty(controllerName))
            {
                operation.Tags = new[] { new OpenApiTag { Name = controllerName } };
            }
        }
    }

    private string GenerateSummary(OperationFilterContext context)
    {
        var actionName = context.MethodInfo.Name;
        var httpMethod = context.ApiDescription.HttpMethod;

        // Generate readable summary from method name
        return httpMethod?.ToUpper() switch
        {
            "GET" when actionName.StartsWith("Get") => $"Retrieve {SplitCamelCase(actionName.Substring(3))}",
            "POST" when actionName.StartsWith("Create") => $"Create new {SplitCamelCase(actionName.Substring(6))}",
            "PUT" when actionName.StartsWith("Update") => $"Update {SplitCamelCase(actionName.Substring(6))}",
            "DELETE" when actionName.StartsWith("Delete") => $"Delete {SplitCamelCase(actionName.Substring(6))}",
            "POST" => $"Execute {SplitCamelCase(actionName)}",
            _ => SplitCamelCase(actionName)
        };
    }

    private string SplitCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return string.Concat(
            input.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString())
        ).ToLower();
    }
}
