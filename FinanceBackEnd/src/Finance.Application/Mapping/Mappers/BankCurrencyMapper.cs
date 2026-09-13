using Finance.Application.Dtos.BankCurrencies;
using Finance.Application.Mapping.Base;
using Finance.Domain.Models.BankCurrencies;

namespace Finance.Application.Mapping.Mappers;

public class BankCurrencyMapper : BaseMapper<BankCurrency, BankCurrencyDto>, IBankCurrencyMapper
{
    public BankCurrencyMapper(IMappingService mappingService) : base(mappingService)
    {
    }
}

public interface IBankCurrencyMapper : IMapper<BankCurrency, BankCurrencyDto>;
