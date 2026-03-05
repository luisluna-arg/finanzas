using Finance.Application.Legacy.Dtos.Base;
using Finance.Application.Legacy.Dtos.Currencies;
using Finance.Domain.Enums;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Legacy.Dtos.Subscriptions;

public record SubscriptionDto : Dto<Guid>
{
    public SubscriptionDto() { }

    public Guid CurrencyId { get; set; }
    public CurrencyDto Currency { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public Money Price { get; set; } = 0;
    public FrequencyEnum Frequency { get; set; } = FrequencyEnum.Monthly;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
