using Finance.Domain.Models;
using Finance.Persistance.TypeConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class CreditCardMovementConfiguration : IEntityTypeConfiguration<CreditCardMovement>
{
    public void Configure(EntityTypeBuilder<CreditCardMovement> builder)
    {
        builder
            .Property(o => o.TimeStamp)
            .HasConversion(o => o.ToUniversalTime(), o => o);

        builder
            .Property(o => o.PlanStart)
            .HasConversion(o => o.ToUniversalTime(), o => o);

        builder
            .Property(o => o.Amount)
            .HasConversion(new MoneyConverter());

        builder
            .Property(o => o.AmountDollars)
            .HasConversion(new MoneyConverter());

        builder
            .HasOne(o => o.CreditCard)
            .WithMany(o => o.Movements);
    }
}
