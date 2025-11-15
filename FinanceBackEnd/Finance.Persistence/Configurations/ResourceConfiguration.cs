using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Debits;
using Finance.Domain.Models.Funds;
using Finance.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class EntityResourceConfiguration<TEntityResource, TEntity, TId> : AuditedEntityConfiguration<TEntityResource, TId>
    where TEntity : Entity<TId>, new()
    where TEntityResource : EntityResource<TEntity, TId>
    where TId : struct
{
    public override void Configure(EntityTypeBuilder<TEntityResource> builder)
    {
        base.Configure(builder);
        builder.HasOne(fr => fr.Resource)
            .WithMany()
            .HasForeignKey(fr => fr.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fr => fr.ResourceSource)
            .WithMany()
            .HasForeignKey(fr => fr.ResourceSourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(fr => new { fr.ResourceId, fr.ResourceSourceId })
            .IsUnique();
    }
}

public class CreditCardResourceConfiguration : EntityResourceConfiguration<CreditCardResource, CreditCard, Guid>;
public class CurrencyExchangeRateResourceConfiguration : EntityResourceConfiguration<CurrencyExchangeRateResource, CurrencyExchangeRate, Guid>;
public class DebitResourceConfiguration : EntityResourceConfiguration<DebitResource, Debit, Guid>;
public class FundResourceConfiguration : EntityResourceConfiguration<FundResource, Fund, Guid>;
public class ResourceConfiguration : AuditedEntityConfiguration<Resource, Guid>;
