using FinanceApi.Domain.Enums;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceApi.Domain.Configurations;

public class AppModuleTypeConfiguration : IEntityTypeConfiguration<AppModuleType>
{
    public void Configure(EntityTypeBuilder<AppModuleType> builder)
    {
        builder
            .Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => (AppModuleTypeEnum)Enum.Parse(typeof(AppModuleTypeEnum), v));
    }
}
