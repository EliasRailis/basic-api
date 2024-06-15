using System.ComponentModel.DataAnnotations.Schema;
using HaefeleSoftware.Api.Domain.Common;

namespace HaefeleSoftware.Api.Domain.Entities;

[Table("tokens")]
public sealed class Token : Audit
{
    public int Id { get; set; }
    
    public required string RefreshToken { get; set; } = null!;
    
    public required DateTime ExpiresAt { get; set; }
    
    public required string CreatedByIp { get; set; } = null!;
    
    public string? RevokedByIp { get; set; } 
    
    public int FK_UserId { get; set; }
    
    public User User { get; set; } = null!;
    
    public bool IsExpired { get; set; }
    
    public bool IsRevoked { get; set; }
}