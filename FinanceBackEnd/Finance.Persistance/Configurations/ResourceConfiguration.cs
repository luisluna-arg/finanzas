using Finance.Domain.Models;
using Finance.Persistance.Configurations.Base;

namespace Finance.Persistance.Configurations;

public class CreditCardMovementResourceConfiguration : AuditedEntityConfiguration<CreditCardMovementResource, Guid>;
public class CreditCardResourceConfiguration : AuditedEntityConfiguration<CreditCardResource, Guid>;
public class CreditCardStatementResourceConfiguration : AuditedEntityConfiguration<CreditCardStatementResource, Guid>;
public class CurrencyExchangeRateResourceConfiguration : AuditedEntityConfiguration<CurrencyExchangeRateResource, Guid>;
public class DebitResourceConfiguration : AuditedEntityConfiguration<DebitResource, Guid>;
public class FundResourceConfiguration : AuditedEntityConfiguration<FundResource, Guid>;
public class ResourceConfiguration : AuditedEntityConfiguration<Resource, Guid>;