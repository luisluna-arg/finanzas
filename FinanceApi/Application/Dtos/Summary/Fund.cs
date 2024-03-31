namespace FinanceApi.Application.Dtos.Summary;

public class Fund
{
    private readonly string id;
    private readonly string label;
    private readonly decimal value;

    public Fund(string id, string label, decimal value)
    {
        this.id = id;
        this.label = label;
        this.value = value;
    }

    public string Id { get => id; }
    public string Label { get => label; }
    public decimal Value { get => value; }
}