﻿using HaefeleSoftware.Api.Application.Interfaces;
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
}