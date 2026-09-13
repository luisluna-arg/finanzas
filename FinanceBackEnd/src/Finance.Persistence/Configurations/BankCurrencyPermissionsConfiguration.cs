using Finance.Domain.Models.Auth;
using Finance.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class BankCurrencyPermissionsConfiguration : AuditedEntityConfiguration<BankCurrencyPermissions, Guid>
{
    public override void Configure(EntityTypeBuilder<BankCurrencyPermissions> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(o => o.BankCurrency)
            .WithMany()
            .HasForeignKey(o => new { o.BankId, o.CurrencyId })
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(o => new { o.BankId, o.CurrencyId, o.UserId })
            .IsUnique();
    }
}
