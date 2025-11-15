using Finance.Application.Dtos.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Dtos.CreditCards;

public record CreditCardTransactionDto : Dto<Guid>
{
    public Guid CreditCardId { get; set; }
    public CreditCardDto? CreditCard { get; set; }
    public Guid? StatementTransactionId { get; set; }
    public CreditCardStatementTransactionDto? StatementTransaction { get; set; }
    public DateTime Timestamp { get; set; }
    public CreditCardTransactionType TransactionType { get; set; }
    public string Concept { get; set; } = string.Empty;
    public Money Amount { get; set; } = 0;
    public string? Reference { get; set; }

    public CreditCardTransactionDto() : base()
    {
    }
}
