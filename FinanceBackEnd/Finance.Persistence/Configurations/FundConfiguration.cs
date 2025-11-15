using Finance.Domain.Models.Funds;
using Finance.Persistence.Configurations.Base;

namespace Finance.Persistence.Configurations;

public class FundConfiguration : AuditedEntityConfiguration<Fund, Guid>;
