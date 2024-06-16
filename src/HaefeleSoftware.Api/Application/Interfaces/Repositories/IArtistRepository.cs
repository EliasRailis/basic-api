using HaefeleSoftware.Api.Domain.Entities;

namespace HaefeleSoftware.Api.Application.Interfaces.Repositories;

public interface IArtistRepository
{
    Task<bool> DoesArtistExistAsync(string name);
    
    Task<bool> AddArtistAsync(Artist artist);
    
    Task<Artist?> GetArtistByIdAsync(int id);
    
    Task<bool> UpdateArtistAsync(Artist artist);
}