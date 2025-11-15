using Finance.Domain.Enums;
using Finance.Domain.Models.AppModules;
using Finance.Persistence.Configurations.Base;

namespace Finance.Persistence.Configurations;

public class AppModuleTypeConfiguration : KeyValueEntityConfiguration<AppModuleType, AppModuleTypeEnum>;
