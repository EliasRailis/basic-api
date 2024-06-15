using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;

namespace HaefeleSoftware.Api.Infrastructure.Services.Jwt;

public sealed class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context, IJwtService jwtService, IUserRepository userRepository)
    {
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?
            .Split(" ")
            .Last();
        
        int? userId = jwtService.ValidateToken(token);
        
        if (userId is not null)
        {
            context.Items["User"] = await userRepository.GetCurrentUserByIdAsync(userId.Value);
        }

        await _next(context);
    }
}

public static class JwtMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<JwtMiddleware>();
    }
}