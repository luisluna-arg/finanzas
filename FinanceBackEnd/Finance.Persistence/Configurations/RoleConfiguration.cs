using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistence.Configurations.Base;

namespace Finance.Persistence.Configurations;

public class RoleConfiguration : KeyValueEntityConfiguration<Role, RoleEnum>;
