using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models.Base;
using FinanceApi.Domain.Models.Interfaces;

namespace FinanceApi.Domain.Models;

public class Income : Entity<Guid>, IAmountHolder
{
    public Guid BankId { get; set; }

    public Guid CurrencyId { get; set; }

    public virtual Bank? Bank { get; set; }

    public virtual Currency? Currency { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime TimeStamp { get; set; }

    public Money Amount { get; set; }
}
