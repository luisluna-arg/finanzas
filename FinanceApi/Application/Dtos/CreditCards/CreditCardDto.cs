using FinanceApi.Application.Dtos.Banks;

namespace FinanceApi.Application.Dtos.CreditCards;

public record CreditCardDto : Dto<Guid>
{
    public CreditCardDto()
        : base()
    {
    }

    public Guid BankId { get; set; }

    public BankDto Bank { get; set; }

    required public string Name { get; set; }
}
