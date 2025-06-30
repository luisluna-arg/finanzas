using System.ComponentModel.DataAnnotations.Schema;
using Finance.Domain.Models.Base;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Models;

public class Movement() : AuditedEntity<Guid>()
{
    [ForeignKey("AppModuleId")]
    public Guid AppModuleId { get; set; }
    [ForeignKey("BankId")]
    public Guid BankId { get; set; }
    public virtual AppModule AppModule { get; set; } = default!;
    public virtual Bank? Bank { get; set; } = default;
    public virtual Currency? Currency { get; set; } = default;
    public Guid? CurrencyId { get; set; } = null;
    required public DateTime TimeStamp { get; set; }
    required public string Concept1 { get; set; }
    required public string? Concept2 { get; set; }
    required public Money Amount { get; set; }
    required public Money? Total { get; set; }
}
