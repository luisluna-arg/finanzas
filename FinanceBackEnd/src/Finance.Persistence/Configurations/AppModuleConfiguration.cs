using Finance.Domain.Models.AppModules;
using Finance.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class AppModuleConfiguration : AuditedEntityConfiguration<AppModule, Guid>
{
    public override void Configure(EntityTypeBuilder<AppModule> builder)
    {
        base.Configure(builder);
        builder
            .HasMany(c => c.Movements)
            .WithOne(e => e.AppModule)
            .IsRequired();

        builder
            .HasOne(c => c.Type);
    }
}
