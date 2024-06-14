using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaefeleSoftware.Api.Application.Interfaces;

public interface IDatabaseContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
    DbSet<User> Users { get; set; }
    
    DbSet<Role> Roles { get; set; }
}