namespace Finance.Application.Dtos.Summary;

public class GeneralSummary : BaseSummaryItem
{
    public GeneralSummary(string id, string label, decimal value)
        : base(id, label, value)
    {
    }
}