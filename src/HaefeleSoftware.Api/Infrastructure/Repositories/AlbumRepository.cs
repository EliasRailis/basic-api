using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaefeleSoftware.Api.Infrastructure.Repositories;

public sealed class AlbumRepository : IAlbumRepository
{
    private readonly IDatabaseContext _context;

    public AlbumRepository(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Artist?> GetArtistAlbumsByIdAsync(int id)
    {
        return await _context.Artists
            .Include(x => x.Albums)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> AddAlbumAsync(Album album)
    {
        _context.Albums.Add(album);
        return await _context.SaveChangesAsync(new CancellationToken()) > 0;
    }

    public Task<Album?> GetAlbumByIdAsync(int id)
    {
        return _context.Albums.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UpdateAlbumAsync(Album album)
    {
        _context.Albums.Update(album);
        return await _context.SaveChangesAsync(new CancellationToken()) > 0;
    }
}