using System.Reflection;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaefeleSoftware.Api.Infrastructure.Persistence;

public sealed class DatabaseContext : DbContext, IDatabaseContext
{
    private readonly IDateTimeService _dateTime;
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options, IDateTimeService dateTime) : base(options)
    {
        _dateTime = dateTime;
    }
    
    public DatabaseContext(IDateTimeService dateTime)
    {
        _dateTime = dateTime;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(DatabaseSettings.DefaultSchema);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (var entry in ChangeTracker.Entries<Audit>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Created = _dateTime.Now;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModified = _dateTime.Now;
                    break;
                case EntityState.Deleted:
                    entry.Entity.LastModified = _dateTime.Now;
                    break;
            }
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<User> Users { get; set; }
    
    public DbSet<Role> Roles { get; set; }
    
    public DbSet<Token> Tokens { get; set; }
    
    public DbSet<Album> Albums { get; set; }
    
    public DbSet<Library> Libraries { get; set; }
    
    public DbSet<Artist> Artists { get; set; }
    
    public DbSet<Song> Songs { get; set; }
    
    public DbSet<LibraryAlbum> LibraryAlbums { get; set; }
}