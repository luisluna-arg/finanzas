using FinanceApi.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FinanceApi.Core.EntityFramework.Converters;

public class InvestmentAssetIOLTypeConverter : ValueConverter<InvestmentAssetIOLTypeEnum, ushort>
{
    public InvestmentAssetIOLTypeConverter(ConverterMappingHints? mappingHints = null)
        : base(
              enumInstance => Convert.ToUInt16(enumInstance),
              uShortInstance => (InvestmentAssetIOLTypeEnum)uShortInstance,
              mappingHints)
    {
    }
}
