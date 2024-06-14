using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HaefeleSoftware.Api.Application.Configurations;

public sealed class Swagger : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public Swagger(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var desc in _provider.ApiVersionDescriptions)
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = $"HaefeleSoftware API v{desc.ApiVersion}",
                Version = $"v{desc.ApiVersion.ToString()}",
            };
            
            options.SwaggerDoc(desc.GroupName, openApiInfo);
        }
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }
}