using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<CreditCardPayment>
{
    public void Configure(EntityTypeBuilder<CreditCardPayment> builder)
    {
        builder.HasKey(p => p.Id);

        builder
            .Property(p => p.Amount)
            .HasColumnType("numeric(18,4)");

        builder
            .HasOne(p => p.CreditCard)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.CreditCardId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Optional relations - enforce FK constraints but restrict deletes
        builder
            .HasOne(p => p.Statement)
            .WithMany()
            .HasForeignKey(p => p.StatementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(p => p.PaymentPlan)
            .WithMany()
            .HasForeignKey(p => p.PaymentPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(p => p.Installment)
            .WithMany()
            .HasForeignKey(p => p.InstallmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
