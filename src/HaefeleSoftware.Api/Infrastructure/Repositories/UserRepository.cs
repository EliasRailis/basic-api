using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;

namespace HaefeleSoftware.Api.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IDatabaseContext _context;

    public UserRepository(IDatabaseContext context)
    {
        _context = context;
    }
}