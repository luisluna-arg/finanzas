using Finance.Domain.SpecialTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Finance.Persistence.TypeConverters;

public class MoneyConverter : ValueConverter<Money, decimal>
{
    public MoneyConverter(ConverterMappingHints? mappingHints = null)
        : base(
            toDb => toDb,
            fromDb => fromDb,
            mappingHints)
    {
    }
}
