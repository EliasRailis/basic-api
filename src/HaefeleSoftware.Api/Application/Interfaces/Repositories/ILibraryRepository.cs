using HaefeleSoftware.Api.Domain.Entities;

namespace HaefeleSoftware.Api.Application.Interfaces.Repositories;

public interface ILibraryRepository
{
    Task<bool> DoesLibraryExistAsync(string name);
    
    Task<bool> AddLibraryAsync(Library library);
    
    Task<Library?> GetLibraryByIdAsync(int id);
    
    Task<bool> UpdateLibraryAsync(Library library);
    
    Task<User?> GetUserLibrariesByIdAsync(int id);
    
    Task<bool> AddAlbumToLibraryAsync(IEnumerable<LibraryAlbum> libraryAlbum);
}