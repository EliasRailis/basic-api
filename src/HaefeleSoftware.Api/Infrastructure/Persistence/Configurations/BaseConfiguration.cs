using HaefeleSoftware.Api.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HaefeleSoftware.Api.Infrastructure.Persistence.Configurations;

public abstract class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : Audit
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(x => x.Created)
            .HasColumnOrder(1)
            .HasColumnName("created")
            .HasColumnType("datetime2")
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(x => x.LastModified)
            .HasColumnOrder(2)
            .HasColumnName("last_modified")
            .HasColumnType("datetime2")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.CreatedBy)
            .HasColumnOrder(3)
            .HasColumnName("created_by")
            .HasColumnType("nvarchar")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.LastModifiedBy)
            .HasColumnOrder(4)
            .HasColumnName("last_modified_by")
            .HasColumnType("nvarchar")
            .HasMaxLength(255)
            .IsRequired(false);
        
        ConfigureRemainingProperties(builder);
    }
    
    protected abstract void ConfigureRemainingProperties(EntityTypeBuilder<T> builder);
}