using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistance.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class IdentityProviderConfiguration : AuditedEntityConfiguration<IdentityProvider, IdentityProviderEnum>
{
    public override void Configure(EntityTypeBuilder<IdentityProvider> builder)
    {
        base.Configure(builder);

        builder
            .Property(e => e.Id)
            .HasConversion(
                v => (short)v,
                v => (IdentityProviderEnum)v)
            .ValueGeneratedNever();

        builder
            .HasData(new IdentityProvider() { Id = IdentityProviderEnum.Auth, Name = IdentityProviderEnum.Auth.ToString() });
    }
}
