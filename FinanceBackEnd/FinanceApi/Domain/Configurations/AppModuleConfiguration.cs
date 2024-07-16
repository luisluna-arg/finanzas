using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceApi.Domain.Configurations;

public class AppModuleConfiguration : IEntityTypeConfiguration<AppModule>
{
    public void Configure(EntityTypeBuilder<AppModule> builder)
    {
        builder
            .HasMany(c => c.Movements)
            .WithOne(e => e.AppModule)
            .IsRequired();

        builder
            .HasOne(c => c.Type);
    }
}
