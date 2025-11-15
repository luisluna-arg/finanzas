using Finance.Domain.Models.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Debits;
using Finance.Domain.Models.Funds;
using Finance.Domain.Models.Incomes;
using Finance.Domain.Models.IOLInvestments;
using Finance.Domain.Models.Movements;

namespace Finance.Domain.Models.Auth;

public class CreditCardResource : EntityResource<CreditCard, Guid>;
public class CurrencyExchangeRateResource : EntityResource<CurrencyExchangeRate, Guid>;
public class DebitOriginResource : EntityResource<DebitOrigin, Guid>;
public class DebitResource : EntityResource<Debit, Guid>;
public class FundResource : EntityResource<Fund, Guid>;
public class IOLInvestmentAssetResource : EntityResource<IOLInvestmentAsset, Guid>;
public class IOLInvestmentResource : EntityResource<IOLInvestment, Guid>;
public class IncomeResource : EntityResource<Income, Guid>;
public class MovementResource : EntityResource<Movement, Guid>;
