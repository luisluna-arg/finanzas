namespace FinanceApi.Application.Dtos.Summary;

public abstract class BaseSummaryItem
{
    protected readonly string id;
    protected readonly string label;
    protected readonly decimal value;

    protected BaseSummaryItem(string id)
    {
        this.id = id;
    }

    protected BaseSummaryItem(string id, string label, decimal value)
    {
        this.id = id;
        this.label = label;
        this.value = value;
    }

    public string Id { get => id; }
    public string Label { get => label; }
    public decimal Value { get => value; }
}