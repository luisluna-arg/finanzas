using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class Debit : Entity<Guid>
{
    public Guid OriginId { get; set; }

    public virtual DebitOrigin Origin { get; set; }

    public DateTime TimeStamp { get; set; } = DateTime.MinValue;

    public Money Amount { get; set; } = 0m;
}
