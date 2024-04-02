namespace FinanceApi.Application.Dtos.Summary;

public class TotalFunds
{
    private List<Fund> funds;

    public TotalFunds()
    {
        funds = new List<Fund>();
    }

    public List<Fund> Funds { get => funds; }

    public void Add(Fund expense)
    {
        this.funds.Add(expense);
        this.funds = this.funds.OrderBy(o => o.Label).ToList();
    }
}