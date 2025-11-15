using Finance.Application.Dtos.Base;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.SpecialTypes;

namespace Finance.Application.Dtos.CreditCards;

public record CreditCardPaymentDto : Dto<Guid>
{
    public Guid StatementId { get; set; }
    public CreditCardStatementDto? Statement { get; set; }
    public DateTime Timestamp { get; set; }
    public Money Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Other;
    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;

    public CreditCardPaymentDto() : base()
    {
    }
}
