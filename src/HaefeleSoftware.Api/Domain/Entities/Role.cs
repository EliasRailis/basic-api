using System.ComponentModel.DataAnnotations.Schema;
using HaefeleSoftware.Api.Domain.Common;

namespace HaefeleSoftware.Api.Domain.Entities;

[Table("roles")]
public sealed class Role : Audit
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public bool IsDeleted { get; set; }

    public ICollection<User> Users { get; set; } = [];
}