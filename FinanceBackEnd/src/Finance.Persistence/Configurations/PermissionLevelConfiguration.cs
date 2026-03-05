using Finance.Domain.Models.Frequencies;
using Finance.Persistence.Configurations.Base;
using FinanceBackEnd.Finance.Domain.Enums;

namespace Finance.Persistence.Configurations;

public class PermissionLevelConfiguration : KeyValueEntityConfiguration<PermissionLevel, PermissionLevelEnum>;
