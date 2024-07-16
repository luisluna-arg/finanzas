using FinanceApi.Core.SpecialTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FinanceApi.Domain.TypeConverters;

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
