using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Domain.Common;

namespace HaefeleSoftware.Api.Infrastructure.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    public CurrentUser User { get; }

    public CurrentUserService(IHttpContextAccessor context)
    {
        User = context.HttpContext?.Items["User"] as CurrentUser ?? new CurrentUser();
    }
}