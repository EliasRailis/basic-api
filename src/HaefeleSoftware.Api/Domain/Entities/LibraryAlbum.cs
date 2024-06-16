using System.ComponentModel.DataAnnotations.Schema;
using HaefeleSoftware.Api.Domain.Common;

namespace HaefeleSoftware.Api.Domain.Entities;

[Table("library_albums")]
public sealed class LibraryAlbum : Audit
{
    public int Id { get; set; }
    
    public int FK_LibraryId { get; set; }

    public Library Library { get; set; } = null!;
    
    public int FK_AlbumId { get; set; }

    public Album Album { get; set; } = null!;
}