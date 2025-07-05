using Finance.Application.Dtos;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Dtos.Debits;
using Finance.Application.Dtos.Funds;
using Finance.Application.Dtos.Incomes;
using Finance.Application.Dtos.IOLInvestments;
using Finance.Application.Dtos.Movements;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

// Consolidated paginated mapper profiles for simple entity mappings
// Organized alphabetically for better maintainability

public class PaginatedCreditCardMovementMapperProfile() : PaginatedResultMapperProfile<CreditCardMovement, CreditCardMovementDto>();
public class PaginatedCurrencyExchangeRateMapperProfile() : PaginatedResultMapperProfile<CurrencyExchangeRate, CurrencyExchangeRateDto>();
public class PaginatedDebitMapperProfile() : PaginatedResultMapperProfile<Debit, DebitDto>();
public class PaginatedFundMapperProfile() : PaginatedResultMapperProfile<Fund, FundDto>();
public class PaginatedIncomeMapperProfile() : PaginatedResultMapperProfile<Income, IncomeDto>();
public class PaginatedIOLInvestmentMapperProfile() : PaginatedResultMapperProfile<IOLInvestment, IOLInvestmentDto>();
public class PaginatedMovementMapperProfile() : PaginatedResultMapperProfile<Movement, MovementDto>();
