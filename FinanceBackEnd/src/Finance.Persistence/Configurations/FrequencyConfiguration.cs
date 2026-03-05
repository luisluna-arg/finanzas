using Finance.Domain.Enums;
using Finance.Domain.Models.Frequencies;
using Finance.Persistence.Configurations.Base;

namespace Finance.Persistence.Configurations;

public class FrequencyConfiguration : KeyValueEntityConfiguration<Frequency, FrequencyEnum>;
