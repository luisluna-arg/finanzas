using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistance.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class IdentityConfiguration : AuditedEntityConfiguration<Identity, Guid>
{
    public override void Configure(EntityTypeBuilder<Identity> builder)
    {
        base.Configure(builder);

        builder
            .Property(o => o.SourceId)
            .HasMaxLength(100);

        builder
            .Property(o => o.Provider)
            .HasConversion(
                v => (short)v,
                v => (IdentityProviderEnum)v);

        builder
            .HasOne(i => i.User)
            .WithMany(u => u.Identities)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
