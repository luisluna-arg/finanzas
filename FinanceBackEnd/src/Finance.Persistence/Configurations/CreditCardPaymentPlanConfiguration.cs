using Finance.Domain.Models.CreditCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CreditCardPaymentPlanConfiguration : IEntityTypeConfiguration<CreditCardPaymentPlan>
{
    public void Configure(EntityTypeBuilder<CreditCardPaymentPlan> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.BaseConcept)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.TotalInstallments)
            .IsRequired();

        builder.HasOne(p => p.CreditCard)
            .WithMany(c => c.PaymentPlans)
            .HasForeignKey(p => p.CreditCardId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Currency)
            .WithMany()
            .HasForeignKey(p => p.CurrencyId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.CreditCardId, p.BaseConcept, p.TotalInstallments, p.CurrencyId });
    }
}
