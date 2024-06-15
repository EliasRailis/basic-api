using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HaefeleSoftware.Api.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : BaseConfiguration<User>
{
    protected override void ConfigureRemainingProperties(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .HasColumnName("id");
        
        builder.Property(x => x.FirstName)
            .HasColumnName("first_name")
            .HasColumnType("nvarchar")
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(x => x.LastName)
            .HasColumnName("last_name")
            .HasColumnType("nvarchar")
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasColumnType("nvarchar")
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(x => x.Password)
            .HasColumnName("password_hash")
            .HasColumnType("nvarchar")
            .HasMaxLength(1000)
            .IsRequired();
        
        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("bit")
            .IsRequired();
        
        builder.Property(x => x.IsDeleted)
            .HasColumnName("is_deleted")
            .HasColumnType("bit")
            .IsRequired();
        
        builder.HasMany(x => x.Tokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.FK_UserId)
            .IsRequired();
        
        builder.Property(x => x.FK_RoleId)
            .HasColumnName("fk_role_id");
    }
}