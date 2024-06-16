using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HaefeleSoftware.Api.Infrastructure.Persistence.Configurations;

public sealed class LibraryAlbumConfiguration : BaseConfiguration<LibraryAlbum>
{
    protected override void ConfigureRemainingProperties(EntityTypeBuilder<LibraryAlbum> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .HasColumnName("id");
    }
}