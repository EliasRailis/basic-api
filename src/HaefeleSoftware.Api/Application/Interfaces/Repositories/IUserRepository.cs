using HaefeleSoftware.Api.Domain.Common;

namespace HaefeleSoftware.Api.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<CurrentUser?> GetCurrentUserByIdAsync(int id);
}