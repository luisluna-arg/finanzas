using Finance.Domain.Models.CreditCards;
using Finance.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CreditCardInstallmentPatternConfiguration : AuditedEntityConfiguration<CreditCardInstallmentPattern, Guid>
{
    public override void Configure(EntityTypeBuilder<CreditCardInstallmentPattern> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.RegexPattern)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => new { p.IsSystem, p.UserId });
    }
}
