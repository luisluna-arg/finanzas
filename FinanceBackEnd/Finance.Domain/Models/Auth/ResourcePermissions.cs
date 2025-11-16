using Finance.Domain.Models.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Debits;
using Finance.Domain.Models.Funds;
using Finance.Domain.Models.Incomes;
using Finance.Domain.Models.Interfaces;
using Finance.Domain.Models.IOLInvestments;
using Finance.Domain.Models.Movements;
using FinanceBackEnd.Finance.Domain.Enums;

namespace Finance.Domain.Models.Auth;

public abstract class ResourcePermissions<TResource, TResourceId>() : AuditedEntity<Guid>()
    where TResource : IEntity
{
    public TResourceId ResourceId { get; set; } = default!;
    public Guid UserId { get; set; } = default!;
    public TResource Resource { get; set; } = default!;
    public User User { get; set; } = default!;
    public ICollection<PermissionLevelEnum> PermissionLevels { get; set; } = [];
}

public class CreditCardPermissions : ResourcePermissions<CreditCard, Guid>;
public class CurrencyExchangeRatePermissions : ResourcePermissions<CurrencyExchangeRate, Guid>;
public class DebitOriginPermissions : ResourcePermissions<DebitOrigin, Guid>;
public class DebitPermissions : ResourcePermissions<Debit, Guid>;
public class FundPermissions : ResourcePermissions<Fund, Guid>;
public class IOLInvestmentAssetPermissions : ResourcePermissions<IOLInvestmentAsset, Guid>;
public class IOLInvestmentPermissions : ResourcePermissions<IOLInvestment, Guid>;
public class IncomePermissions : ResourcePermissions<Income, Guid>;
public class MovementPermissions : ResourcePermissions<Movement, Guid>;
