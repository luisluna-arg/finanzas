namespace FinanceApi.Application.Dtos.CreditCards;

public record CreditCardIssuerDto : Dto<Guid>
{
    public CreditCardIssuerDto()
        : base()
    {
    }

    public Guid BankId { get; set; }
    required public string Name { get; set; }
}
