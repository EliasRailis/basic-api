using System.ComponentModel.DataAnnotations.Schema;
using HaefeleSoftware.Api.Domain.Common;

namespace HaefeleSoftware.Api.Domain.Entities;

[Table("users")]
public sealed class User : Audit
{
    public int Id { get; set; }
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string Email { get; set; }
    
    public required string Password { get; set; }
    
    public int FK_RoleId { get; set; }

    public Role Role { get; set; } = null!;
    
    public bool IsActive { get; set; }
    
    public bool IsDeleted { get; set; }
}