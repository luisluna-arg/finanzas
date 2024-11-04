using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistance.TypeConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class DebitConfiguration : IEntityTypeConfiguration<Debit>
{
    public void Configure(EntityTypeBuilder<Debit> builder)
    {
        builder.Property(e => e.TimeStamp)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        builder
            .Property(o => o.Amount)
            .HasConversion(new MoneyConverter());

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
