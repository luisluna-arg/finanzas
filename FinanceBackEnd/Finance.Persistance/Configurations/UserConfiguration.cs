using Finance.Domain.Models;
using Finance.Persistance.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

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

        // Configure many-to-many relationship through UserRole junction table
        builder
            .HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity<UserRole>(
                j => j.HasOne(ur => ur.Role)
                      .WithMany()
                      .HasForeignKey(ur => ur.RoleId),
                j => j.HasOne(ur => ur.User)
                      .WithMany()
                      .HasForeignKey(ur => ur.UserId),
                j => j.HasKey(ur => ur.Id));
    }
}
