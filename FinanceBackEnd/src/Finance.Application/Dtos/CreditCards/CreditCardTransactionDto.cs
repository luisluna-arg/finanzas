using Finance.Application.Dtos.Base;
using Finance.Application.Dtos.Currencies;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Interfaces;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Dtos.CreditCards;

public record CreditCardTransactionDto : Dto<Guid>, IAmountHolder
{
    public Guid CreditCardId { get; set; }
    public CreditCardDto? CreditCard { get; set; }
    public Guid? StatementTransactionId { get; set; }
    public CreditCardStatementTransactionDto? StatementTransaction { get; set; }
    public DateTime Timestamp { get; set; }
    public CreditCardTransactionType TransactionType { get; set; }
    public string Concept { get; set; } = string.Empty;
    public Money Amount { get; set; } = 0;
    public Money ConvertedAmount { get; set; } = 0;
    public string? Reference { get; set; }
    public Guid CurrencyId { get; set; }
    public CurrencyDto? Currency { get; set; }

    public CreditCardTransactionDto() : base()
    {
    }
}
