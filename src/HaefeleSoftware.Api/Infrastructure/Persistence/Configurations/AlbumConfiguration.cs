using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HaefeleSoftware.Api.Infrastructure.Persistence.Configurations;

public sealed class AlbumConfiguration : BaseConfiguration<Album>
{
    protected override void ConfigureRemainingProperties(EntityTypeBuilder<Album> builder)
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
        
        builder.Property(x => x.YearOfRelease)
            .HasColumnName("year_of_release")
            .HasColumnType("nvarchar")
            .HasMaxLength(5)
            .IsRequired();
        
        builder.Property(x => x.Duration)
            .HasColumnName("duration")
            .HasColumnType("nvarchar")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.NumberOfSongs)
            .HasColumnName("number_of_songs")
            .HasColumnType("int");
        
        builder.Property(x => x.IsDeleted)
            .HasColumnName("is_deleted")
            .HasColumnType("bit")
            .IsRequired();
        
        builder.Property(x => x.FK_ArtistId)
            .HasColumnName("fk_artist_id");
        
        builder.HasMany(x => x.LibraryAlbums)
            .WithOne(x => x.Album)
            .HasForeignKey(x => x.FK_AlbumId)
            .IsRequired();
        
        builder.HasMany(x => x.Songs)
            .WithOne(x => x.Album)
            .HasForeignKey(x => x.FK_AlbumId)
            .IsRequired();
    }
}