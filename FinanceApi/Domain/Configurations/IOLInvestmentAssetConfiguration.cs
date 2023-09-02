using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceApi.Domain.Configurations;

public class IOLInvestmentAssetConfiguration : IEntityTypeConfiguration<IOLInvestmentAsset>
{
    public void Configure(EntityTypeBuilder<IOLInvestmentAsset> builder)
    {
        // Method intentionally left empty.
    }
}
