using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class Bank : Entity<Guid>
{
    public Bank()
        : base()
    {
    }

    required public string? Name { get; set; }
}
