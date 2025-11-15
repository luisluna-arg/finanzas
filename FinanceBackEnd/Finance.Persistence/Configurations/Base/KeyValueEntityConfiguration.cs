using Finance.Domain.Models.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations.Base;

public abstract class KeyValueEntityConfiguration<T, TEnum> : AuditedEntityConfiguration<T, TEnum>
    where T : KeyValueEntity<TEnum, T>, new()
    where TEnum : struct, Enum
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder
            .Property(e => e.Id)
            .HasConversion(
                v => Convert.ToInt16(v),
                v => (TEnum)Enum.ToObject(typeof(TEnum), v))
            .ValueGeneratedNever();

        // Use anonymous objects for seeding data to avoid the required property issue
        var seedData = Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(e => new
            {
                Id = e,
                Name = e.ToString(),
                Deactivated = false,
                CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

        builder.HasData(seedData);
    }
}
