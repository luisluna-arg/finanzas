namespace Finance.Application.Dtos.Summary;

public class Fund : BaseSummaryItem
{
    public string BaseCurrency { get; }
    public string BaseCurrencySymbol { get; }
    public Guid BaseCurrencyId { get; }
    public decimal QuoteCurrencyValue { get; }
    public string DefaultCurrency { get; }
    public string DefaultCurrencySymbol { get; }
    public Guid DefaultCurrencyId { get; }    public Fund(
        string id,
        string label,
        decimal value,
        Guid baseCurrencyId,
        string baseCurrency,
        string baseCurrencySymbol,
        decimal quoteCurrencyValue,
        Guid quoteCurrencyId,
        string quoteCurrency,
        string quoteCurrencySymbol)
    : base(id, label, value)
    {
        BaseCurrencyId = baseCurrencyId;
        BaseCurrency = baseCurrency;
        BaseCurrencySymbol = baseCurrencySymbol;
        QuoteCurrencyValue = quoteCurrencyValue;
        DefaultCurrency = quoteCurrency;
        DefaultCurrencySymbol = quoteCurrencySymbol;
        DefaultCurrencyId = quoteCurrencyId;
    }
}