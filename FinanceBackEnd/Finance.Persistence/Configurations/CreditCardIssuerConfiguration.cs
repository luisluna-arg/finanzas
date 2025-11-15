using Finance.Domain.Models.CreditCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CreditCardIssuerConfiguration : IEntityTypeConfiguration<CreditCardIssuer>
{
    public void Configure(EntityTypeBuilder<CreditCardIssuer> builder)
    {
        builder.HasKey(i => i.Id);

        builder
            .Property(i => i.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .HasIndex(i => i.Code)
            .IsUnique();
    }
}
