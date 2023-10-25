using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class AppModule : Entity<Guid>
{
    required public string Name { get; set; } = string.Empty;

    required public DateTime CreatedAt { get; set; }

    required public virtual Currency Currency { get; set; }

    public virtual ICollection<Movement> Movements { get; set; } = new List<Movement>();
}
