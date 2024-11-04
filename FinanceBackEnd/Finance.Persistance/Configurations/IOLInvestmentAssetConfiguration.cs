using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class IOLInvestmentAssetConfiguration : IEntityTypeConfiguration<IOLInvestmentAsset>
{
    public void Configure(EntityTypeBuilder<IOLInvestmentAsset> builder)
    {
        // Method intentionally left empty.
    }
}
