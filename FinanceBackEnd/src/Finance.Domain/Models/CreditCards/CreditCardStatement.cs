using Finance.Domain.Models.Base;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Models.CreditCards;

public class CreditCardStatement : CreditCardEntity
{
    public DateTime ClosureDate { get; set; }
    public DateTime ExpiringDate { get; set; }
    public Money MinimumDue { get; set; }
}
