namespace FinanceApi.Application.Dtos.Summary;

public class TotalFunds
{
    private readonly List<Fund> fund;

    public TotalFunds()
    {
        fund = new List<Fund>();
    }

    public List<Fund> Funds { get => fund; }

    public void Add(Fund expense)
    {
        this.fund.Add(expense);
    }
}