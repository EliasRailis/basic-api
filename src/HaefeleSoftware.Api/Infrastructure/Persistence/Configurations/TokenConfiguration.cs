using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HaefeleSoftware.Api.Infrastructure.Persistence.Configurations;

public sealed class TokenConfiguration : BaseConfiguration<Token>
{
    protected override void ConfigureRemainingProperties(EntityTypeBuilder<Token> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .HasColumnName("id");
        
        builder.Property(x => x.RefreshToken)
            .HasColumnName("refresh_token")
            .HasColumnType("nvarchar")
            .HasMaxLength(1500)
            .IsRequired();
        
        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .HasColumnType("datetime")
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(x => x.CreatedByIp)
            .HasColumnName("created_by_ip")
            .HasColumnType("nvarchar")
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(x => x.RevokedByIp)
            .HasColumnName("revoked_by_ip")
            .HasColumnType("nvarchar")
            .HasMaxLength(255)
            .IsRequired(false);
        
        builder.Property(x => x.IsDeleted)
            .HasColumnName("is_deleted")
            .HasColumnType("bit")
            .IsRequired();
        
        builder.Property(x => x.IsExpired)
            .HasColumnName("is_expired")
            .HasColumnType("bit")
            .IsRequired();
        
        builder.Property(x => x.IsRevoked)
            .HasColumnName("is_revoked")
            .HasColumnType("bit")
            .IsRequired();

        builder.Property(x => x.FK_UserId)
            .HasColumnName("fk_user_id");
    }
}