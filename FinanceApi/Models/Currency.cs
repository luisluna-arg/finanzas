namespace FinanceApi.Models;

internal class Currency : Entity
{
    public Currency()
        : base()
    {
    }

    required public string Name { get; set; } = string.Empty;
}
