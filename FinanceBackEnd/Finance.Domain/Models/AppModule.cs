using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class AppModule : AuditedEntity<Guid>
{
    public AppModule() { }

    required public string Name { get; set; } = string.Empty;
    required public virtual Currency Currency { get; set; }
    required public virtual AppModuleType Type { get; set; }
    public virtual ICollection<Movement> Movements { get; set; } = [];
}
