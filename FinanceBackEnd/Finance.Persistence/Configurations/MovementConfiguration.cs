using Finance.Domain.Models.Movements;
using Finance.Persistence.Configurations.Base;

namespace Finance.Persistence.Configurations;

public class MovementConfiguration : AuditedEntityConfiguration<Movement, Guid>;
