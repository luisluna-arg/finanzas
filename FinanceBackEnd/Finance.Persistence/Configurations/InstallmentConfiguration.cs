using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class InstallmentConfiguration : IEntityTypeConfiguration<CreditCardInstallment>
{
    public void Configure(EntityTypeBuilder<CreditCardInstallment> builder)
    {
        builder.HasKey(i => i.Id);

        builder
            .Property(i => i.Amount)
            .HasColumnType("numeric(18,4)");

        builder
            .Property(i => i.PaidAmount)
            .HasColumnType("numeric(18,4)");

        builder
            .HasOne(i => i.Payment)
            .WithMany()
            .HasForeignKey(i => i.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
