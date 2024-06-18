using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaefeleSoftware.Api.Infrastructure.Repositories;

public sealed class ArtistRepository : IArtistRepository
{
    private readonly IDatabaseContext _context;

    public ArtistRepository(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<bool> DoesArtistExistAsync(string name)
    {
        return await _context.Artists.AnyAsync(x => x.Name == name.Trim());
    }

    public async Task<bool> AddArtistAsync(Artist artist)
    {
        _context.Artists.Add(artist);
        return await _context.SaveChangesAsync(new CancellationToken()) > 0;
    }

    public async Task<Artist?> GetArtistByIdAsync(int id)
    {
        return await _context.Artists.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UpdateArtistAsync(Artist artist)
    {
        _context.Artists.Update(artist);
        return await _context.SaveChangesAsync(new CancellationToken()) > 0;
    }
}