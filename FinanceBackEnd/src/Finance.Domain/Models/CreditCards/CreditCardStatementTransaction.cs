using Finance.Domain.Models.Base;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Models.CreditCards;

public class CreditCardStatementTransaction : CreditCardStatementEntity
{
    public Guid? CreditCardTransactionId { get; set; }
    public virtual CreditCardTransaction? CreditCardTransaction { get; set; }
    public DateTime PostedDate { get; set; }
    public Money Amount { get; set; } = 0;
    public string Description { get; set; } = string.Empty;
}
