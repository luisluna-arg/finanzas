using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CreditCardResource : EntityResource<CreditCard, Guid>;
public class CurrencyExchangeRateResource : EntityResource<CurrencyExchangeRate, Guid>;
public class DebitOriginResource : EntityResource<DebitOrigin, Guid>;
public class DebitResource : EntityResource<Debit, Guid>;
public class FundResource : EntityResource<Fund, Guid>;
public class IOLInvestmentAssetResource : EntityResource<IOLInvestmentAsset, Guid>;
public class IOLInvestmentResource : EntityResource<IOLInvestment, Guid>;
public class IncomeResource : EntityResource<Income, Guid>;
public class MovementResource : EntityResource<Movement, Guid>;
