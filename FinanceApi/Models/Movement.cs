namespace FinanceApi.Models;

internal class Movement : Entity
{
    required public Module Module { get; set; }
    required public DateTime TimeStamp { get; set; }
    required public decimal Ammount { get; set; }
}
