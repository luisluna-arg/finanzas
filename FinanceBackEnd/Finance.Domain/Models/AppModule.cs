using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class AppModule : AuditedEntity<Guid>
{
    public AppModule() { }

    public string Name { get; set; } = string.Empty;
    public virtual Currency Currency { get; set; } = default!;
    public virtual AppModuleType Type { get; set; } = default!;
    public virtual ICollection<Movement> Movements { get; set; } = [];
}
