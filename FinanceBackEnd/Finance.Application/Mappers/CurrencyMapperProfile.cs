using Finance.Application.Dtos.Currencies;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

public class CurrencyMapperProfile() : BaseEntityMapperProfile<Currency, CurrencyDto>();
