using Finance.Domain.Models;
using Finance.Persistance.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class IdentityConfiguration : AuditedEntityConfiguration<Identity, Guid>
{
    public override void Configure(EntityTypeBuilder<Identity> builder)
    {
        base.Configure(builder);

        builder
            .Property(o => o.UserId)
            .HasMaxLength(100);
    }
}
