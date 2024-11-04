using Finance.Domain.SpecialTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Finance.Api.Core.EntityFramework.Converters;

public class MoneyValueConverter : ValueConverter<Money, decimal>
{
    public MoneyValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
              moneyInstance => moneyInstance.Value,
              decimalInstance => decimalInstance,
              mappingHints)
    {
    }
}
