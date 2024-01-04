using System.ComponentModel.DataAnnotations.Schema;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class DebitOrigin : Entity<Guid>
{
    [ForeignKey("AppModuleId")]
    public Guid AppModuleId { get; set; }

    public virtual AppModule AppModule { get; set; }

    required public string Name { get; set; }

    public virtual ICollection<Debit> Debits { get; set; } = new List<Debit>();
}
