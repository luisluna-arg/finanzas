using Finance.Application.Dtos.Base;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Dtos.CreditCards;

public record CreditCardStatementTransactionDto : Dto<Guid>
{
    public Guid CreditCardStatementId { get; set; }
    public CreditCardStatementDto? CreditCardStatement { get; set; }
    public Guid? CreditCardTransactionId { get; set; }
    public CreditCardTransactionDto? CreditCardTransaction { get; set; }
    public DateTime PostedDate { get; set; }
    public Money Amount { get; set; } = 0;
    public string Description { get; set; } = string.Empty;

    public CreditCardStatementTransactionDto() : base()
    {
    }
}
