using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class PaymentAllocationConfiguration : IEntityTypeConfiguration<CreditCardPaymentAllocation>
{
    public void Configure(EntityTypeBuilder<CreditCardPaymentAllocation> builder)
    {
        builder.HasKey(p => p.Id);

        builder
            .Property(p => p.Amount)
            .HasColumnType("numeric(18,4)");

        builder
            .HasOne(p => p.Payment)
            .WithMany()
            .HasForeignKey(p => p.PaymentId)
            .IsRequired();
    }
}
