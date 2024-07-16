using FinanceApi.Core.SpecialTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FinanceApi.Core.EntityFramework.Converters;

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
