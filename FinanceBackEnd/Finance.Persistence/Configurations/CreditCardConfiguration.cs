using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
{
    public void Configure(EntityTypeBuilder<CreditCard> builder)
    {
        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .HasOne(c => c.Bank)
            .WithMany(b => b.CreditCards)
            .HasForeignKey(c => c.BankId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(c => c.CreditCardIssuer)
            .WithMany(i => i.CreditCards)
            .HasForeignKey(c => c.CreditCardIssuerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(c => c.CurrentStatement)
            .WithMany()
            .HasForeignKey(c => c.CurrentStatementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(c => new { c.BankId, c.Name });
    }
}
