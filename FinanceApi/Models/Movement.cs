namespace FinanceApi.Models;

internal class Movement : Entity
{
    public Movement()
        : base()
    {
    }

    required public Module Module { get; set; }
    required public DateTime TimeStamp { get; set; }
    required public decimal Ammount { get; set; }
    required public decimal? Total { get; set; }
    required public string Concept1 { get; set; }
    required public string? Concept2 { get; set; }
}
