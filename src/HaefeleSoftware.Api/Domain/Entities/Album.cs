using System.ComponentModel.DataAnnotations.Schema;
using HaefeleSoftware.Api.Domain.Common;

namespace HaefeleSoftware.Api.Domain.Entities;

[Table("albums")]
public sealed class Album : Audit
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string YearOfRelease { get; set; }
    
    public required string Duration { get; set; }
    
    public int NumberOfSongs { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public int FK_ArtistId { get; set; }

    public Artist Artist { get; set; } = null!;

    public ICollection<LibraryAlbum> LibraryAlbums { get; set; } = [];

    public ICollection<Song> Songs { get; set; } = [];
}