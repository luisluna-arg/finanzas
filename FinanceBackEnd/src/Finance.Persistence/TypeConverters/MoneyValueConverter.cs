using Finance.Domain.SpecialTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Finance.Persistence.TypeConverters;

public class MoneyValueConverter : ValueConverter<Money, decimal>
{
    public MoneyValueConverter() : this(null)
    {
    }

    public MoneyValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
              moneyInstance => moneyInstance.Value,
              decimalInstance => decimalInstance,
              mappingHints)
    {
    }
}

public class NullableMoneyValueConverter : ValueConverter<Money?, decimal?>
{
    public NullableMoneyValueConverter() : this(null)
    {
    }

    public NullableMoneyValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
              moneyInstance => moneyInstance,
              decimalInstance => decimalInstance.HasValue ? new Money(decimalInstance.Value) : null,
              mappingHints)
    {
    }
}
