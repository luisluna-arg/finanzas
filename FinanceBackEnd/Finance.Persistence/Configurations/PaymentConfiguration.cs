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
            .IsRequired();

        builder
            .Property(p => p.Timestamp)
            .IsRequired();

        builder
            .Property(p => p.Method)
            .IsRequired();

        builder
            .Property(p => p.Status)
            .IsRequired();

        builder
            .HasOne(p => p.Statement)
            .WithMany()
            .HasForeignKey(p => p.StatementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(p => p.CreditCard)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.CreditCardId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(p => new { p.CreditCardId, p.Timestamp });
    }
}
