using System.ComponentModel.DataAnnotations.Schema;

namespace HaefeleSoftware.Api.Domain.Common;

public class Audit
{
    public required DateTime Created { get; set; }
    
    public required string CreatedBy { get; set; } 
    
    public DateTime? LastModified { get; set; }
    
    public string? LastModifiedBy { get; set; }
}