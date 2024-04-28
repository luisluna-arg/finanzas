using FinanceApi.Core.SpecialTypes;

namespace FinanceApi.Domain.Models.Interfaces;

public interface IAmountHolder
{
    public Guid CurrencyId { get; set; }
    public Money Amount { get; set; }
}