namespace Finance.Application.Dtos.Summary;

public class GeneralSummary : BaseSummaryItem
{
    public decimal ConvertedValue { get; set; } = 0M;

    public GeneralSummary()
    {
    }
}
