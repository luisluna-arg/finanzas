using Finance.Application.Legacy.Dtos.Base;

namespace Finance.Application.Legacy.Dtos.CreditCards;

public record CreditCardStatementDto : Dto<Guid>
{
    public CreditCardStatementDto() { }

    public Guid CreditCardId { get; set; } = Guid.Empty;
    public CreditCardDto CreditCard { get; set; } = default!;
    public DateTime ClosureDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiringDate { get; set; } = DateTime.UtcNow;
}
