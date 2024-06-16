﻿using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HaefeleSoftware.Api.Infrastructure.Persistence.Configurations;

public sealed class LibraryConfiguration : BaseConfiguration<Library>
{
    protected override void ConfigureRemainingProperties(EntityTypeBuilder<Library> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .HasColumnName("id");
        
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("nvarchar")
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(x => x.IsDeleted)
            .HasColumnName("is_deleted")
            .HasColumnType("bit")
            .IsRequired();
        
        builder.Property(x => x.FK_UserId)
            .HasColumnName("fk_user_id");
        
        builder.HasMany(x => x.LibraryAlbums)
            .WithOne(x => x.Library)
            .HasForeignKey(x => x.FK_LibraryId)
            .IsRequired();
    }
}