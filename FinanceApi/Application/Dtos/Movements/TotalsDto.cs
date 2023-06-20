namespace FinanceApi.Application.Dtos.Movements;

public class TotalsDto
{
    public TotalsDto()
    {
    }

    public DateTime TimeStamp { get; set; } = DateTime.Now;
    public decimal Income { get; set; } = 0;
    public decimal Funds { get; set; } = 0;
    public decimal LastDeposit { get; set; } = 0;
}
