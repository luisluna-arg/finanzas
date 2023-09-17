using FinanceApi.Core.SpecialTypes;

namespace FinanceApi.Application.Dtos.Debits;

public record DebitDto : Dto<Guid>
{
    public DebitDto()
        : base()
    {
    }

    public Guid DebitOriginId { get; set; }

    public DebitOriginDto DebitOrigin { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public Money Amount { get; set; }
}
