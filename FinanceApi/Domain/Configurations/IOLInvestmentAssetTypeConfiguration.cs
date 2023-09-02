using FinanceApi.Domain.Enums;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceApi.Domain.Configurations;

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
