﻿using System.Reflection;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Domain.Common;
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
}