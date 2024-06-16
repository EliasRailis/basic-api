using HaefeleSoftware.Api.Domain.Entities;

namespace HaefeleSoftware.Api.Application.Interfaces.Repositories;

public interface ISongRepository
{
    Task<Album?> GetAlbumSongsAsync(int albumId);
    
    Task<bool> AddSongsAsync(IEnumerable<Song> songs);
    
    Task<Song?> GetSongByIdAsync(int songId);
    
    Task<bool> UpdateSongAsync(Song song);
}