using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CreditCardTransactionConfiguration : IEntityTypeConfiguration<CreditCardTransaction>
{
    public void Configure(EntityTypeBuilder<CreditCardTransaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder
            .Property(t => t.Timestamp)
            .IsRequired();

        builder
            .Property(t => t.TransactionType)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(t => t.Concept)
            .IsRequired()
            .HasMaxLength(500);

        builder
            .Property(t => t.Amount)
            .IsRequired();

        builder
            .Property(t => t.Reference)
            .HasMaxLength(200);

        builder
            .HasOne(t => t.CreditCard)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CreditCardId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(t => t.StatementTransaction)
            .WithMany()
            .HasForeignKey(t => t.StatementTransactionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasIndex(t => new { t.CreditCardId, t.Timestamp });
    }
}
