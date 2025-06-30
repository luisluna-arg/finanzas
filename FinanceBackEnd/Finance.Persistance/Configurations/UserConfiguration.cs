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
    }
}
