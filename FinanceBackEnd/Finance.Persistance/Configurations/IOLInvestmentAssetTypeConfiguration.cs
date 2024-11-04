using Finance.Domain.Enums;
using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class IOLInvestmentAssetTypeConfiguration : IEntityTypeConfiguration<IOLInvestmentAssetType>
{
    public void Configure(EntityTypeBuilder<IOLInvestmentAssetType> builder)
    {
        builder
            .HasData(Enum.GetValues(typeof(IOLInvestmentAssetTypeEnum))
            .Cast<IOLInvestmentAssetTypeEnum>()
            .Select(e => new IOLInvestmentAssetType
            {
                Id = (ushort)e,
                Name = e.ToString()
            }));
    }
}
