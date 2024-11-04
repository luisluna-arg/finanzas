using Finance.Domain.Enums;
using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class FrequencyConfiguration : IEntityTypeConfiguration<Frequency>
{
    public void Configure(EntityTypeBuilder<Frequency> builder)
    {
        builder
            .Property(e => e.Id)
            .HasConversion(
                v => (short)v,
                v => (FrequencyEnum)v)
            .ValueGeneratedNever();

        builder
            .HasData(
                new Frequency() { Id = (short)FrequencyEnum.Monthly, Name = FrequencyEnum.Monthly.ToString() },
                new Frequency() { Id = FrequencyEnum.Annual, Name = FrequencyEnum.Annual.ToString() });
    }
}
