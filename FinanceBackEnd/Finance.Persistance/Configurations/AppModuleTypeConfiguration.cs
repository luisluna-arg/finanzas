using Finance.Domain.Enums;
using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

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
