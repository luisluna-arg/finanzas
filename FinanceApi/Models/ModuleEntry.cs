namespace FinanceApi.Models;

internal class ModuleEntry : Entity
{
    public ModuleEntry() : base()
    {
    }

    public DateTime TimeStamp { get; set; }
    public decimal Ammount { get; set; }
}
