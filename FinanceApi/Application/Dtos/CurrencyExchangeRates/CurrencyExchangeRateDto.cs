namespace FinanceApi.Application.Dtos;

public record CurrencyExchangeRateDto : Dto<Guid>
{
    public CurrencyExchangeRateDto()
        : base()
    {
    }

    public Guid BaseCurrencyId { get; set; }

    public Guid QuoteCurrencyId { get; set; }

    public decimal BuyRate { get; set; }

    public decimal SellRate { get; set; }

    public DateTime TimeStamp { get; set; }
}
