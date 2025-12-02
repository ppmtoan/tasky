using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Volo.Abp.Modularity;

namespace ModuleTest.Shared.Hosting.AspNetCore;

public static class SwaggerConfigurationHelper
{
    public static void Configure(
        ServiceConfigurationContext context,
        string apiTitle
    )
    {
        context.Services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = apiTitle, Version = "v1" });
            options.DocInclusionPredicate((docName, description) => true);
            options.CustomSchemaIds(type => type.FullName);
            
            ConfigureSwaggerOptions(options);
        });
    }
    
    public static void ConfigureWithOidc(
        ServiceConfigurationContext context,
        string authority,
        string[] scopes,
        string apiTitle,
        string apiVersion = "v1",
        string apiName = "v1",
        string[]? flows = null,
        string? discoveryEndpoint = null
    )
    {
        context.Services.AddAbpSwaggerGenWithOidc(
            authority: authority,
            scopes: scopes,
            flows: flows,
            discoveryEndpoint: discoveryEndpoint,
            options =>
            {
                options.SwaggerDoc(apiName, new OpenApiInfo { Title = apiTitle, Version = apiVersion });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
                
                ConfigureSwaggerOptions(options);
            });
    }
    
    private static void ConfigureSwaggerOptions(SwaggerGenOptions options)
    {
        // Enable XML comments for better documentation
        var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
        foreach (var xmlFile in xmlFiles)
        {
            try
            {
                options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
            }
            catch
            {
                // Ignore if XML file cannot be loaded
            }
        }
        
        // Use data annotations for validation rules in Swagger
        options.EnableAnnotations();
        
        // Show enum values as strings
        options.UseInlineDefinitionsForEnums();
        
        // Add support for nullable reference types
        options.SupportNonNullableReferenceTypes();
        
        // Order actions by relative path then HTTP method
        options.OrderActionsBy(apiDesc => $"{apiDesc.RelativePath}_{apiDesc.HttpMethod}");
        
        // Add schema filter to include examples and better descriptions
        options.SchemaFilter<SwaggerSchemaFilter>();
        
        // Add operation filter for better request/response examples
        options.OperationFilter<SwaggerOperationFilter>();
    }
}
