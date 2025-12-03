using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Falcon.Api.Swagger;

/// <summary>
/// Configures Swagger generation to respect API versioning and security requirements.
/// </summary>
public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            var info = new OpenApiInfo
            {
                Title = "Falcon Monitoring API",
                Version = description.ApiVersion.ToString(),
                Description = "Falcon monitoring platform HTTP API endpoints",
            };

            if (description.IsDeprecated)
            {
                info.Description += " (deprecated)";
            }

            options.SwaggerDoc(description.GroupName, info);
        }

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
            Description = "Provide the JWT token prefixed by 'Bearer '."
        });

        var bearerReference = new OpenApiSecuritySchemeReference("Bearer", hostDocument: null, externalResource: null);
        options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
        {
            [bearerReference] = []
        });

        options.CustomSchemaIds(type => type.FullName?.Replace('+', '.') ?? type.Name);
    }
}