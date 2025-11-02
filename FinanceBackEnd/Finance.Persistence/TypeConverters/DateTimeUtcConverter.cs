using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Finance.Persistence.TypeConverters;

public class DateTimeUtcConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeUtcConverter() : this(null)
    {
    }

    public DateTimeUtcConverter(ConverterMappingHints? mappingHints = null)
        : base(
            toDb => toDb.ToUniversalTime(),
            fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc),
            mappingHints)
    {
    }
}

public class NullableDateTimeUtcConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableDateTimeUtcConverter() : this(null)
    {
    }

    public NullableDateTimeUtcConverter(ConverterMappingHints? mappingHints = null)
        : base(
            toDb => toDb.HasValue ? toDb.Value.ToUniversalTime() : (DateTime?)null,
            fromDb => fromDb.HasValue ? DateTime.SpecifyKind(fromDb.Value, DateTimeKind.Utc) : (DateTime?)null,
            mappingHints)
    {
    }
}