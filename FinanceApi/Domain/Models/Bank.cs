using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

public class Bank : Entity
{
    public Bank()
        : base()
    {
    }

    required public string? Name { get; set; }
}
