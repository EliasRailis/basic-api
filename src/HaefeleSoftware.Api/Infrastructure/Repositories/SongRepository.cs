using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaefeleSoftware.Api.Infrastructure.Repositories;

public sealed class SongRepository : ISongRepository
{
    private readonly IDatabaseContext _context;

    public SongRepository(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Album?> GetAlbumSongsAsync(int albumId)
    {
        return await _context.Albums
            .Include(x => x.Songs)
            .Where(x => x.Id == albumId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> AddSongsAsync(IEnumerable<Song> songs)
    {
        _context.Songs.AddRange(songs);
        return await _context.SaveChangesAsync(new CancellationToken()) > 0;
    }

    public async Task<Song?> GetSongByIdAsync(int songId)
    {
        return await _context.Songs.FirstOrDefaultAsync(x => x.Id == songId);
    }

    public async Task<bool> UpdateSongAsync(Song song)
    {
        _context.Songs.Update(song);
        return await _context.SaveChangesAsync(new CancellationToken()) > 0;
    }
}