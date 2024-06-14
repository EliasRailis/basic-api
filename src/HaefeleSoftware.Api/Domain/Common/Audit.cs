using System.ComponentModel.DataAnnotations.Schema;

namespace HaefeleSoftware.Api.Domain.Common;

public class Audit
{
    [Column(Order = 1)]
    public required DateTime Created { get; set; }
    
    [Column(Order = 2)]
    public required string CreatedBy { get; set; } 
    
    [Column(Order = 3)]
    public DateTime? LastModified { get; set; }
    
    [Column(Order = 4)]
    public string? LastModifiedBy { get; set; }
}