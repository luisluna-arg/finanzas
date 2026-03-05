using System.ComponentModel.DataAnnotations.Schema;
using Finance.Domain.Models.AppModules;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models.Debits;

public class DebitOrigin() : Entity<Guid>()
{
    [ForeignKey("AppModuleId")]
    public Guid AppModuleId { get; set; }
    public virtual AppModule AppModule { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<Debit> Debits { get; set; } = [];
}
