using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class AppModule : Entity<Guid>
{
    required public string Name { get; set; } = string.Empty;

    required public DateTime CreatedAt { get; set; }

    required public virtual Currency Currency { get; set; }

    required public virtual AppModuleType Type { get; set; }

    public virtual ICollection<Movement> Movements { get; set; } = new List<Movement>();
}
