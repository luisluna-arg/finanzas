namespace FinanceApi.Application.Models;

public class Currency : Entity
{
    public Currency()
        : base()
    {
    }

    required public string Name { get; set; } = string.Empty;
    required public string ShortName { get; set; } = string.Empty;

    public static Currency Default()
    {
        return new Currency()
        {
            ShortName = string.Empty,
            Name = string.Empty
        };
    }
}
