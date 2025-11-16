using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Debits;
using Finance.Domain.Models.Funds;
using Finance.Domain.Models.Incomes;
using Finance.Domain.Models.IOLInvestments;
using Finance.Domain.Models.Movements;
using Finance.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class ResourcePermissionsConfiguration<TPermissions, TResource, TId> : AuditedEntityConfiguration<TPermissions, Guid>
    where TResource : Entity<TId>, new()
    where TPermissions : ResourcePermissions<TResource, Guid>
    where TId : struct
{
    public override void Configure(EntityTypeBuilder<TPermissions> builder)
    {
        base.Configure(builder);

        builder.HasOne(b => b.Resource)
            .WithMany()
            .HasForeignKey(b => b.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(fr => new { fr.ResourceId, fr.UserId })
            .IsUnique();
    }
}

public class CreditCardPermissionsConfiguration : ResourcePermissionsConfiguration<CreditCardPermissions, CreditCard, Guid>;
public class CurrencyExchangeRatePermissionsConfiguration : ResourcePermissionsConfiguration<CurrencyExchangeRatePermissions, CurrencyExchangeRate, Guid>;
public class DebitOriginPermissionsConfiguration : ResourcePermissionsConfiguration<DebitOriginPermissions, DebitOrigin, Guid>;
public class DebitPermissionsConfiguration : ResourcePermissionsConfiguration<DebitPermissions, Debit, Guid>;
public class FundPermissionsConfiguration : ResourcePermissionsConfiguration<FundPermissions, Fund, Guid>;
public class IOLInvestmentAssetPermissionsConfiguration : ResourcePermissionsConfiguration<IOLInvestmentAssetPermissions, IOLInvestmentAsset, Guid>;
public class IOLInvestmentPermissionsConfiguration : ResourcePermissionsConfiguration<IOLInvestmentPermissions, IOLInvestment, Guid>;
public class IncomePermissionsConfiguration : ResourcePermissionsConfiguration<IncomePermissions, Income, Guid>;
public class MovementPermissionsConfiguration : ResourcePermissionsConfiguration<MovementPermissions, Movement, Guid>;
