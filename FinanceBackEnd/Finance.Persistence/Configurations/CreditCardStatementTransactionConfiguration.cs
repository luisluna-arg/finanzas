using Finance.Domain.Models;
using Finance.Persistence.TypeConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CreditCardStatementTransactionConfiguration : IEntityTypeConfiguration<CreditCardStatementTransaction>
{
    public void Configure(EntityTypeBuilder<CreditCardStatementTransaction> builder)
    {
        builder.HasKey(st => st.Id);

        builder
            .Property(st => st.PostedDate)
            .IsRequired()
            .HasConversion(d => d.ToUniversalTime(), d => d);

        builder
            .Property(st => st.Amount)
            .IsRequired()
            .HasConversion(new MoneyConverter());

        builder
            .Property(st => st.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder
            .HasOne(st => st.CreditCardStatement)
            .WithMany()
            .HasForeignKey(st => st.CreditCardStatementId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(st => st.CreditCardTransaction)
            .WithMany()
            .HasForeignKey(st => st.CreditCardTransactionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasIndex(st => new { st.CreditCardStatementId, st.PostedDate });
    }
}
