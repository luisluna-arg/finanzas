using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceApi.Domain.Configurations;

public class DebitOriginConfiguration : IEntityTypeConfiguration<DebitOrigin>
{
    public void Configure(EntityTypeBuilder<DebitOrigin> builder)
    {
        builder
            .HasIndex(o => o.Name).IsUnique();
    }
}
