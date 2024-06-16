using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HaefeleSoftware.Api.Infrastructure.Persistence.Configurations;

public sealed class SongConfiguration : BaseConfiguration<Song>
{
    protected override void ConfigureRemainingProperties(EntityTypeBuilder<Song> builder)
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
        
        builder.Property(x => x.Duration)
            .HasColumnName("duration")
            .HasColumnType("nvarchar")
            .HasMaxLength(10)
            .IsRequired();
        
        builder.Property(x => x.IsDeleted)
            .HasColumnName("is_deleted")
            .HasColumnType("bit")
            .IsRequired();
    }
}