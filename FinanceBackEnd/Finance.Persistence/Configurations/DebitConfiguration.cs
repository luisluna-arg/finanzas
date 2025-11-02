using Finance.Domain.Enums;
using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class DebitConfiguration : IEntityTypeConfiguration<Debit>
{
    public void Configure(EntityTypeBuilder<Debit> builder)
    {
        builder
            .Property(t => t.Frequency)
            .HasConversion(
                v => (short)v,
                v => (FrequencyEnum)v)
            .HasDefaultValue(FrequencyEnum.Monthly);

        builder
            .HasOne<Frequency>()
            .WithMany()
            .HasForeignKey(t => (short)t.Frequency)
            .IsRequired();
    }
}
