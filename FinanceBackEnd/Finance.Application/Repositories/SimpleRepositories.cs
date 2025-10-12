using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Domain.Enums;
using Finance.Persistence;

namespace Finance.Application.Repositories;

// Consolidated simple repository classes
// Organized alphabetically for better maintainability

// App Module related repositories
public class AppModuleTypeRepository(FinanceDbContext dbContext) : BaseRepository<AppModuleType, AppModuleTypeEnum>(dbContext);

// Bank related repositories
public class BankRepository(FinanceDbContext dbContext) : BaseRepository<Bank, Guid>(dbContext);

// Credit Card related repositories
public class CreditCardMovementRepository(FinanceDbContext dbContext) : BaseRepository<CreditCardMovement, Guid>(dbContext);
public class CreditCardMovementResourceRepository(FinanceDbContext dbContext) : BaseRepository<CreditCardMovement, Guid>(dbContext);
public class CreditCardRepository(FinanceDbContext dbContext) : BaseRepository<CreditCard, Guid>(dbContext);
public class CreditCardResourceRepository(FinanceDbContext dbContext) : BaseRepository<CreditCard, Guid>(dbContext);
public class CreditCardStatementRepository(FinanceDbContext dbContext) : BaseRepository<CreditCardStatement, Guid>(dbContext);
public class CreditCardStatementResourceRepository(FinanceDbContext dbContext) : BaseRepository<CreditCardStatement, Guid>(dbContext);

// Currency related repositories
public class CurrencyConversionRepository(FinanceDbContext dbContext) : BaseRepository<CurrencyConversion, Guid>(dbContext);
public class CurrencyExchangeRateRepository(FinanceDbContext dbContext) : BaseRepository<CurrencyExchangeRate, Guid>(dbContext);
public class CurrencyExchangeRateResourceRepository(FinanceDbContext dbContext) : BaseRepository<CurrencyExchangeRate, Guid>(dbContext);
public class CurrencySymbolRepository(FinanceDbContext dbContext) : BaseRepository<CurrencySymbol, Guid>(dbContext);

// Debit related repositories
public class DebitOriginRepository(FinanceDbContext dbContext) : BaseRepository<DebitOrigin, Guid>(dbContext);
public class DebitOriginResourceRepository(FinanceDbContext dbContext) : BaseRepository<DebitOrigin, Guid>(dbContext);
public class DebitRepository(FinanceDbContext dbContext) : BaseRepository<Debit, Guid>(dbContext);
public class DebitResourceRepository(FinanceDbContext dbContext) : BaseRepository<Debit, Guid>(dbContext);

// Other entity repositories
public class FrequencyRepository(FinanceDbContext dbContext) : BaseRepository<Frequency, FrequencyEnum>(dbContext);
public class FundRepository(FinanceDbContext dbContext) : BaseRepository<Fund, Guid>(dbContext);
public class FundResourceRepository(FinanceDbContext dbContext) : BaseRepository<Fund, Guid>(dbContext);
public class IdentityProviderRepository(FinanceDbContext dbContext) : BaseRepository<IdentityProvider, IdentityProviderEnum>(dbContext);
public class IdentityRepository(FinanceDbContext dbContext) : BaseRepository<Identity, Guid>(dbContext);
public class IncomeRepository(FinanceDbContext dbContext) : BaseRepository<Income, Guid>(dbContext);
public class IncomeResourceRepository(FinanceDbContext dbContext) : BaseRepository<Income, Guid>(dbContext);

// IOL Investment related repositories
public class IOLInvestmentAssetRepository(FinanceDbContext dbContext) : BaseRepository<IOLInvestmentAsset, Guid>(dbContext);
public class IOLInvestmentAssetResourceRepository(FinanceDbContext dbContext) : BaseRepository<IOLInvestmentAsset, Guid>(dbContext);
public class IOLInvestmentAssetTypeRepository(FinanceDbContext dbContext) : BaseRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum>(dbContext);
public class IOLInvestmentAssetTypeResourceRepository(FinanceDbContext dbContext) : BaseRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum>(dbContext);
public class IOLInvestmentRepository(FinanceDbContext dbContext) : BaseRepository<IOLInvestment, Guid>(dbContext);
public class IOLInvestmentResourceRepository(FinanceDbContext dbContext) : BaseRepository<IOLInvestment, Guid>(dbContext);

// Movement related repositories
public class MovementRepository(FinanceDbContext dbContext) : BaseRepository<Movement, Guid>(dbContext);
public class MovementResourceRepository(FinanceDbContext dbContext) : BaseRepository<Movement, Guid>(dbContext);

// Resource related repositories
public class ResourceOwnerRepository(FinanceDbContext dbContext) : BaseRepository<ResourceOwner, Guid>(dbContext);
public class ResourceRepository(FinanceDbContext dbContext) : BaseRepository<Resource, Guid>(dbContext);

// User related repositories
public class RoleRepository(FinanceDbContext dbContext) : BaseRepository<Role, RoleEnum>(dbContext);
public class UserRepository(FinanceDbContext dbContext) : BaseRepository<User, Guid>(dbContext);
