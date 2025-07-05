namespace Finance.Application.Dtos.Summary;

public abstract class BaseSummaryItem
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; } = 0M;
    
    protected BaseSummaryItem()
    {
    }
}