using Finance.Application.Dtos;
using Finance.Application.Dtos.AppModules;
using Finance.Application.Dtos.Banks;
using Finance.Application.Dtos.CreditCards;
using Finance.Application.Dtos.Currencies;
using Finance.Application.Dtos.CurrencyConversions;
using Finance.Application.Dtos.Frequencies;
using Finance.Application.Dtos.Funds;
using Finance.Application.Dtos.Identities;
using Finance.Application.Dtos.Incomes;
using Finance.Application.Dtos.IOLInvestmentAssets;
using Finance.Application.Dtos.IOLInvestmentAssetTypes;
using Finance.Application.Dtos.IOLInvestments;
using Finance.Application.Dtos.Movements;
using Finance.Application.Dtos.Users;
using Finance.Application.Mappers.Base;
using Finance.Domain.Models;

namespace Finance.Application.Mappers;

// Consolidated mapper profiles for simple entity mappings
// Organized alphabetically for better maintainability

// App Module related mappers
public class AppModuleBasicMapperProfile() : BaseEntityMapperProfile<AppModule, AppModuleBasicDto>();
public class AppModuleMapperProfile() : BaseEntityMapperProfile<AppModule, AppModuleDto>();
public class AppModuleTypeMapperProfile() : BaseEntityMapperProfile<AppModuleType, AppModuleTypeDto>();

// Bank related mappers
public class BankMapperProfile() : BaseEntityMapperProfile<Bank, BankDto>();

// Credit Card related mappers
public class CreditCardMovementMapperProfile() : BaseEntityMapperProfile<CreditCardMovement, CreditCardMovementDto>();
public class CreditCardStatementMapperProfile() : BaseEntityMapperProfile<CreditCardStatement, CreditCardStatementDto>();

// Currency related mappers
public class CurrencyConversionMapperProfile() : BaseEntityMapperProfile<CurrencyConversion, CurrencyConversionDto>();
public class CurrencyExchangeRateMapperProfile() : BaseEntityMapperProfile<CurrencyExchangeRate, CurrencyExchangeRateDto>();
public class CurrencyMapperProfile() : BaseEntityMapperProfile<Currency, CurrencyDto>();

// Other entity mappers
public class FrequencyMapperProfile() : BaseEntityMapperProfile<Frequency, FrequencyDto>();
public class FundMapperProfile() : BaseEntityMapperProfile<Fund, FundDto>();
public class IdentityMapperProfile() : BaseEntityMapperProfile<Identity, IdentityDto>();
public class IncomeMapperProfile() : BaseEntityMapperProfile<Income, IncomeDto>();

// IOL Investment related mappers
public class IOLInvestmentAssetMapperProfile() : BaseEntityMapperProfile<IOLInvestmentAsset, IOLInvestmentAssetDto>();
public class IOLInvestmentAssetTypeMapperProfile() : BaseEntityMapperProfile<IOLInvestmentAssetType, IOLInvestmentAssetTypeDto>();
public class IOLInvestmentMapperProfile() : BaseEntityMapperProfile<IOLInvestment, IOLInvestmentDto>();

// Movement and User related mappers
public class MovementMapperProfile() : BaseEntityMapperProfile<Movement, MovementDto>();
public class RoleMapperProfile() : BaseEntityMapperProfile<Role, RoleDto>();
public class UserMapperProfile() : BaseEntityMapperProfile<User, UserDto>();
