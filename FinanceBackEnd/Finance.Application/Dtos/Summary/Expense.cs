namespace Finance.Application.Dtos.Summary;

public class Expense : BaseSummaryItem
{
    public Expense(string id, string label, decimal value)
        : base(id, label, value)
    {
    }
}