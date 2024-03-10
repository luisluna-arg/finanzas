namespace FinanceApi.Application.Dtos.Summary;

public class TotalInvestments
{
    private readonly List<Investment> investment;
    public TotalInvestments()
    {
        investment = new List<Investment>();
    }

    public List<Investment> Investments { get => investment; }

    public void Add(Investment expense)
    {
        this.investment.Add(expense);
    }
}