using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class PaymentPlanConfiguration : IEntityTypeConfiguration<CreditCardPaymentPlan>
{
    public void Configure(EntityTypeBuilder<CreditCardPaymentPlan> builder)
    {
        builder.HasKey(p => p.Id);

        builder
            .HasMany(p => p.Installments)
            .WithOne(i => i.PaymentPlan)
            .HasForeignKey(i => i.PaymentPlanId)
            .IsRequired();

        builder
            .HasOne(p => p.CreditCard)
            .WithMany(c => c.PaymentPlans)
            .HasForeignKey(p => p.CreditCardId)
            .IsRequired();

        builder
            .Property(p => p.TotalAmount)
            .HasColumnType("numeric(18,4)");
    }
}
