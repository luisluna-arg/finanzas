using Finance.Domain.Models.CreditCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CreditCardStatementImportTemplateConfiguration : IEntityTypeConfiguration<CreditCardStatementImportTemplate>
{
    public void Configure(EntityTypeBuilder<CreditCardStatementImportTemplate> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.ConfigJson)
            .IsRequired();

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.CreditCards)
            .WithMany(c => c.ImportTemplates)
            .UsingEntity(j => j.ToTable("CreditCardImportTemplate"));

        builder.HasIndex(t => new { t.IsSystem, t.UserId });
    }
}
