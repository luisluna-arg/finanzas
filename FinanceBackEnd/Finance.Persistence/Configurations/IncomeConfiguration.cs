using Finance.Domain.Models;
using Finance.Persistence.Configurations.Base;

namespace Finance.Persistence.Configurations;

public class IncomeConfiguration : AuditedEntityConfiguration<Income, Guid>;
