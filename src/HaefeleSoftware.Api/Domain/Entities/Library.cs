using System.ComponentModel.DataAnnotations.Schema;
using HaefeleSoftware.Api.Domain.Common;

namespace HaefeleSoftware.Api.Domain.Entities;

[Table("libraries")]
public sealed class Library : Audit
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public bool IsDeleted { get; set; }
    
    public int FK_UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<LibraryAlbum> LibraryAlbums { get; set; } = [];
}