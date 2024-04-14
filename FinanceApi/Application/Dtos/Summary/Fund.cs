namespace FinanceApi.Application.Dtos.Summary;

public class Fund : BaseSummaryItem
{
    public Fund(string id, string label, decimal value)
        : base(id, label, value)
    {
    }
}