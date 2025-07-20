using Finance.Domain.Enums;
using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class IOLInvestmentAssetConfiguration : IEntityTypeConfiguration<IOLInvestmentAsset>
{
    public void Configure(EntityTypeBuilder<IOLInvestmentAsset> builder)
    {
        builder.Property(e => e.TypeId)
            .HasConversion(
                v => (short)v,
                v => (IOLInvestmentAssetTypeEnum)v
            )
            .HasColumnType("smallint");
    }
}
