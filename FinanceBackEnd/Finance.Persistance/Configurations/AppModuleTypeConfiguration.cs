using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistance.Configurations.Base;

namespace Finance.Persistance.Configurations;

public class AppModuleTypeConfiguration : KeyValueEntityConfiguration<AppModuleType, AppModuleTypeEnum>;
