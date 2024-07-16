namespace FinanceApi.Application.Dtos.Summary;

public class TotalGeneralSummary : BaseSummaryTotals<GeneralSummary>
{
    public TotalGeneralSummary()
        : base()
    {
    }

    public TotalGeneralSummary(ICollection<GeneralSummary> items)
        : base(items)
    {
    }
}