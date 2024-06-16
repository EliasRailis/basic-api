using HaefeleSoftware.Api.Domain.Entities;

namespace HaefeleSoftware.Api.Application.Interfaces.Repositories;

public interface IAlbumRepository
{
    Task<Artist?> GetArtistAlbumsByIdAsync(int id);
    
    Task<bool> AddAlbumAsync(Album album);
    
    Task<Album?> GetAlbumByIdAsync(int id);
    
    Task<bool> UpdateAlbumAsync(Album album);
}