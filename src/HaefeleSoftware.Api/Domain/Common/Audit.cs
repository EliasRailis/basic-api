namespace HaefeleSoftware.Api.Domain.Common;

public class Audit
{
    public DateTime Created { get; set; }
    
    public string CreatedBy { get; set; } = default!; 
    
    public DateTime? LastModified { get; set; }
    
    public string? LastModifiedBy { get; set; }
}