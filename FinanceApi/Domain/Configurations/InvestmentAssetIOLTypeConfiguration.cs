using FinanceApi.Domain.Enums;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceApi.Domain.Configurations;

public class InvestmentAssetIOLTypeConfiguration : IEntityTypeConfiguration<InvestmentAssetIOLType>
{
    public void Configure(EntityTypeBuilder<InvestmentAssetIOLType> builder)
    {
        builder
            .HasData(Enum.GetValues(typeof(InvestmentAssetIOLTypeEnum))
            .Cast<InvestmentAssetIOLTypeEnum>()
            .Select(e => new InvestmentAssetIOLType
            {
                Id = (short)e,
                Name = e.ToString()
            }));
    }
}
