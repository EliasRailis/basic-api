using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.Builder;
using HaefeleSoftware.Api.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HaefeleSoftware.Api.Application.Configurations;

public static class Endpoints
{
    public static void AddEndpoints(this IServiceCollection service, Assembly assembly)
    {
        ServiceDescriptor[] descriptors = assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();
        
        service.TryAddEnumerable(descriptors);
    }

    public static void MapEndpoints(this WebApplication app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        RouteGroupBuilder routeGroupBuilder = app.MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();
        IEndpointRouteBuilder builder = routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoints(builder);
        }
    }
}