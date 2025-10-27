using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class BankConfiguration : IEntityTypeConfiguration<Bank>
{
    public void Configure(EntityTypeBuilder<Bank> builder)
    {
        builder.HasKey(b => b.Id);

        builder
            .Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .HasIndex(b => b.Name)
            .IsUnique();
    }
}
