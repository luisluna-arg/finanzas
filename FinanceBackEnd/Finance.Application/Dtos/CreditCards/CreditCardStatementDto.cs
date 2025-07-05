namespace Finance.Application.Dtos.CreditCards;

public record CreditCardStatementDto() : Dto<Guid>
{
    public Guid CreditCardId { get; set; }

    public CreditCardDto CreditCard { get; set; } = default!;

    public DateTime ClosureDate { get; set; }

    public DateTime ExpiringDate { get; set; }
}
