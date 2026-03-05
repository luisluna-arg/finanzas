using Finance.Domain.Enums;
using Finance.Domain.Models.Base;
using Finance.Domain.Models.Currencies;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Models.Subscriptions;

public class Subscription() : AuditedEntity<Guid>()
{
    public Guid CurrencyId { get; set; }
    public Currency Currency { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public Money Price { get; set; } = 0;
    public FrequencyEnum Frequency { get; set; } = FrequencyEnum.Monthly;
}
