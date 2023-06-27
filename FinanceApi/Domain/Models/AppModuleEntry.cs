namespace FinanceApi.Domain.Models;

public class AppModuleEntry : Entity
{
    public AppModuleEntry()
        : base()
    {
    }

    public DateTime TimeStamp { get; set; }
    public decimal Ammount { get; set; }
}
