
using CQRSDispatch.Interfaces;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Services.Requests.CurrencyExchangeRates;

public class DeleteCurrencyExchangeRateSagaRequest : ICommand, ISagaRequest
{
    public Guid Id { get; }

    public DeleteCurrencyExchangeRateSagaRequest(Guid id) : base()
    {
        Id = id;
    }
}
