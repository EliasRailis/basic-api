using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaefeleSoftware.Api.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IDatabaseContext _context;

    public UserRepository(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<CurrentUser?> GetCurrentUserByIdAsync(int id)
    {
        return await _context.Users
            .Where(x => x.Id == id)
            .Select(x => new CurrentUser
            {
                Id = x.Id,
                Email = x.Email,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<bool> DoesEmailExistAsync(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email.Trim());
    }

    public async Task<bool> AddUserAsync(User user)
    {
        _context.Users.Add(user);
        return await _context.SaveChangesAsync(new CancellationToken()) > 0;
    }
}