using System.ComponentModel.DataAnnotations.Schema;
using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class Debit : Entity<Guid>
{
    [ForeignKey("DebitOriginId")]
    public Guid DebitOriginId { get; set; }

    public virtual DebitOrigin DebitOrigin { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public Money Amount { get; set; }
}
