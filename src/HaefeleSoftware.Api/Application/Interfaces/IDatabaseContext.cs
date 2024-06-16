using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaefeleSoftware.Api.Application.Interfaces;

public interface IDatabaseContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
    DbSet<User> Users { get; set; }
    
    DbSet<Role> Roles { get; set; }
    
    DbSet<Token> Tokens { get; set; }
    
    DbSet<Album> Albums { get; set; }
    
    DbSet<Library> Libraries { get; set; }
    
    DbSet<Artist> Artists { get; set; }
    
    DbSet<Song> Songs { get; set; }
    
    DbSet<LibraryAlbum> LibraryAlbums { get; set; }
}