using FinanceApi.Core.SpecialTypes;

namespace FinanceApi.Application.Dtos.CreditCards;

public record CreditCardMovementDto : Dto<Guid>
{
    public CreditCardMovementDto()
        : base()
    {
    }

    public Guid CreditCardIssuerId { get; set; }
    required public DateTime TimeStamp { get; set; }
    required public DateTime PlanStart { get; set; }
    required public string Concept { get; set; }
    required public short PaymentNumber { get; set; } = 1;
    required public short PlanSize { get; set; } = 1;
    required public Money Amount { get; set; } = 0;
    required public Money AmountDollars { get; set; } = 0;
}
