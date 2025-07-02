using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistance.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class UserRoleConfiguration : AuditedEntityConfiguration<UserRole, Guid>
{
    public override void Configure(EntityTypeBuilder<UserRole> builder)
    {
        base.Configure(builder);

        builder
            .Property(o => o.RoleId)
            .HasConversion(
                v => (short)v,
                v => (RoleEnum)v);
    }
}
