using System.ComponentModel.DataAnnotations.Schema;
using HaefeleSoftware.Api.Domain.Common;

namespace HaefeleSoftware.Api.Domain.Entities;

[Table("songs")]
public sealed class Song : Audit
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Duration { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public int FK_AlbumId { get; set; }

    public Album Album { get; set; } = null!;
}