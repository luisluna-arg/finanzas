using Finance.Application.Legacy.Dtos.Base;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Legacy.Dtos.CreditCards;

public record CreditCardMovementDto : Dto<Guid>
{
    public Guid CreditCardId { get; set; }
    public DateTime TimeStamp { get; set; }
    public DateTime PlanStart { get; set; }
    public string Concept { get; set; } = string.Empty;
    public short PaymentNumber { get; set; } = 1;
    public short PlanSize { get; set; } = 1;
    public Money Amount { get; set; } = 0;
    public Money AmountDollars { get; set; } = 0;

    public CreditCardMovementDto() : base()
    {
    }
}
