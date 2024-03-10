namespace FinanceApi.Application.Dtos.Summary;

public class Investment
{
    private readonly string id;
    private readonly string symbol;
    private readonly decimal averageReturn;
    private readonly decimal valued;

    public Investment(string id, string symbol, decimal averageReturn, decimal valued)
    {
        this.id = id;
        this.symbol = symbol;
        this.averageReturn = averageReturn;
        this.valued = valued;
    }

    public string Id { get => id; }
    public string Symbol { get => symbol; }
    public decimal AverageReturn { get => averageReturn; }
    public decimal Valued { get => valued; }
}