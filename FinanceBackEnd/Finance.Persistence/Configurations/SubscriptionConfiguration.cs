using Finance.Domain.Enums;
using Finance.Domain.Models.Frequencies;
using Finance.Domain.Models.Subscriptions;
using Finance.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class SubscriptionConfiguration : AuditedEntityConfiguration<Subscription, Guid>
{
    public override void Configure(EntityTypeBuilder<Subscription> builder)
    {
        base.Configure(builder);

        builder
            .HasOne(o => o.Currency)
            .WithMany(o => o.Subscriptions)
            .IsRequired();

        builder
            .HasIndex(o => o.Name)
            .IsUnique();

        builder
            .HasIndex(o => o.Price)
            .IsUnique();

        builder
            .Property(t => t.Frequency)
            .HasConversion(
                v => (short)v,
                v => (FrequencyEnum)v)
            .HasDefaultValue(FrequencyEnum.Monthly);

        builder
            .HasOne<Frequency>()
            .WithMany()
            .HasForeignKey(t => (short)t.Frequency)
            .IsRequired();
    }
}
