using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaefeleSoftware.Api.Infrastructure.Repositories;

public sealed class LibraryRepository : ILibraryRepository
{
    private readonly IDatabaseContext _context;

    public LibraryRepository(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<bool> DoesLibraryExistAsync(string name)
    {
        return await _context.Libraries.AnyAsync(x => x.Name == name.Trim());
    }

    public async Task<bool> AddLibraryAsync(Library library)
    {
        _context.Libraries.Add(library);
        return await _context.SaveChangesAsync(new CancellationToken()) > 0;
    }

    public async Task<Library?> GetLibraryByIdAsync(int id)
    {
        return await _context.Libraries.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UpdateLibraryAsync(Library library)
    {
        _context.Libraries.Update(library);
        return await _context.SaveChangesAsync(new CancellationToken()) > 0;
    }

    public async Task<User?> GetUserLibrariesByIdAsync(int id)
    {
        return await _context.Users
            .Include(x => x.Libraries)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
    }
}