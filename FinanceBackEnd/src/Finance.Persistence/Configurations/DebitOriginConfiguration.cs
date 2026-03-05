using Finance.Domain.Models.Debits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class DebitOriginConfiguration : IEntityTypeConfiguration<DebitOrigin>
{
    public void Configure(EntityTypeBuilder<DebitOrigin> builder)
    {
        builder
            .HasIndex(o => new { o.Name, o.AppModuleId }).IsUnique();

        builder
            .HasMany(o => o.Debits)
            .WithOne(o => o.Origin)
            .IsRequired();
    }
}
