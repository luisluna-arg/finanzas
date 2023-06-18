namespace FinanceApi.Application.Models;

public class Bank : Entity
{
    public Bank()
        : base()
    {
    }

    required public string? Name { get; set; }
}
