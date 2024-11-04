using Finance.Domain.SpecialTypes;
using Finance.Domain.Enums;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class Debit : Entity<Guid>
{
    public Guid OriginId { get; set; }

    public virtual DebitOrigin Origin { get; set; }

    public DateTime TimeStamp { get; set; } = DateTime.MinValue;

    public Money Amount { get; set; } = 0m;

    public FrequencyEnum Frequency { get; set; }
}