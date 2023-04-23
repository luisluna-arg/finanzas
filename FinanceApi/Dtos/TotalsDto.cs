namespace FinanceApi.Dtos;

public class TotalsDto
{
    public TotalsDto()
    {
    }

    public DateTime TimeStamp { get; internal set; } = DateTime.Now;
    public decimal Income { get; internal set; } = 0;
    public decimal Funds { get; internal set; } = 0;
    public decimal LastDeposit { get; internal set; } = 0;
}