using Finance.Domain.Enums;
using Finance.Domain.Models.Base;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Models;

public class Debit() : Entity<Guid>()
{
    public Guid OriginId { get; set; }
    public virtual DebitOrigin Origin { get; set; } = default!;
    public DateTime TimeStamp { get; set; } = DateTime.MinValue;
    public Money Amount { get; set; } = 0m;
    public FrequencyEnum Frequency { get; set; }
}
