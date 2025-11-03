namespace Finance.Application.Dtos.Summary;

public class Fund : BaseSummaryItem
{
    public string BaseCurrency { get; set; } = string.Empty;
    public string BaseCurrencySymbol { get; set; } = string.Empty;
    public Guid BaseCurrencyId { get; set; } = Guid.Empty;
    public decimal QuoteCurrencyValue { get; set; } = 0M;
    public string DefaultCurrency { get; set; } = string.Empty;
    public string DefaultCurrencySymbol { get; set; } = string.Empty;
    public Guid DefaultCurrencyId { get; set; } = Guid.Empty;

    public Fund()
    {
    }
}
