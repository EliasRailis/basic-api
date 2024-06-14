using HaefeleSoftware.Api.Application.Common.Behaviors;
using MediatR;

namespace HaefeleSoftware.Api.Application.Configurations;

public static class Behaviors
{
    public static void AddBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Validation<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Performance<,>));
    }
}