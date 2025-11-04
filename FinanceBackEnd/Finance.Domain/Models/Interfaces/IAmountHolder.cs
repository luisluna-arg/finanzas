using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Models.Interfaces;

public interface IAmountHolder
{
    public Guid CurrencyId { get; set; }
    public Money Amount { get; set; }
}
