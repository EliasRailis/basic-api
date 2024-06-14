using HaefeleSoftware.Api.Domain.Common;

namespace HaefeleSoftware.Api.Application.Interfaces;

public interface ICurrentUserService
{
    CurrentUser User { get; }
}