namespace Finance.Application.Dtos.Summary;

public class Investment : BaseSummaryItem
{
    public string Symbol { get; set; } = string.Empty;
    public decimal AverageReturn { get; set; } = 0M;
    public decimal Valued { get; set; } = 0M;

    public Investment()
    {
    }
}
