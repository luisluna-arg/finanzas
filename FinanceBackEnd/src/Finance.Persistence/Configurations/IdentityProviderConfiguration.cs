using Finance.Domain.Enums;
using Finance.Domain.Models.Identities;
using Finance.Persistence.Configurations.Base;

namespace Finance.Persistence.Configurations;

public class IdentityProviderConfiguration : KeyValueEntityConfiguration<IdentityProvider, IdentityProviderEnum>;
