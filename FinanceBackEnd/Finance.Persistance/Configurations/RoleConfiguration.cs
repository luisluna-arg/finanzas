using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistance.Configurations.Base;

namespace Finance.Persistance.Configurations;

public class RoleConfiguration : KeyValueEntityConfiguration<Role, RoleEnum>;
