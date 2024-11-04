namespace Finance.Application.Dtos.CreditCards;

public record CreditCardStatementDto : Dto<Guid>
{
    public CreditCardStatementDto()
        : base()
    {
    }

    public Guid CreditCardId { get; set; }

    public CreditCardDto CreditCard { get; set; }

    public DateTime ClosureDate { get; set; }

    public DateTime ExpiringDate { get; set; }
}
