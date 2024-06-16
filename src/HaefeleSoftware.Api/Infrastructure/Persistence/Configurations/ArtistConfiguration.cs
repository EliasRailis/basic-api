using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HaefeleSoftware.Api.Infrastructure.Persistence.Configurations;

public sealed class ArtistConfiguration : BaseConfiguration<Artist>
{
    protected override void ConfigureRemainingProperties(EntityTypeBuilder<Artist> builder)
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
        
        builder.HasMany(x => x.Albums)
            .WithOne(x => x.Artist)
            .HasForeignKey(x => x.FK_ArtistId)
            .IsRequired();
    }
}