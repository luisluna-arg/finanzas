using System.ComponentModel.DataAnnotations.Schema;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class DebitOrigin : Entity<Guid>
{
    [ForeignKey("AppModuleId")]
    public Guid AppModuleId { get; set; }

    public virtual AppModule AppModule { get; set; }

    required public string Name { get; set; }

    public virtual ICollection<Debit> Debits { get; set; } = new List<Debit>();
}
