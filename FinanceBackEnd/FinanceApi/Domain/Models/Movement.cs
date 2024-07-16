using System.ComponentModel.DataAnnotations.Schema;
using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class Movement : Entity<Guid>
{
    [ForeignKey("AppModuleId")]
    public Guid AppModuleId { get; set; }

    [ForeignKey("BankId")]
    public Guid BankId { get; set; }

    public virtual AppModule AppModule { get; set; }

    public virtual Bank? Bank { get; set; }

    public virtual Currency? Currency { get; set; }

    public Guid? CurrencyId { get; set; } = null;

    required public DateTime CreatedAt { get; set; }

    required public DateTime TimeStamp { get; set; }

    required public string Concept1 { get; set; }

    required public string? Concept2 { get; set; }

    required public Money Amount { get; set; }

    required public Money? Total { get; set; }
}
