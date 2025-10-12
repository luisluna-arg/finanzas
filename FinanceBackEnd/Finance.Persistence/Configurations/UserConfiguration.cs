using Finance.Domain.Models;
using Finance.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class UserConfiguration : AuditedEntityConfiguration<User, Guid>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder
            .Property(o => o.FirstName)
            .HasMaxLength(100);

        builder
            .Property(o => o.LastName)
            .HasMaxLength(100);

        builder
            .Property(o => o.Username)
            .HasMaxLength(100);

        builder
            .HasIndex(o => o.Username)
            .IsUnique();

        // Configure many-to-many relationship for User <-> Role with custom key type
        builder
            .HasMany(u => u.Roles)
            .WithMany() // Removed (r => r.Users) since Role does not have a Users navigation property
            .UsingEntity<Dictionary<string, object>>(
                "UserRole",
                j => j
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .HasPrincipalKey("Id")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .HasPrincipalKey("Id")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("UserId", "RoleId");
                    j.Property("RoleId").HasColumnType("smallint");
                });
    }
}
