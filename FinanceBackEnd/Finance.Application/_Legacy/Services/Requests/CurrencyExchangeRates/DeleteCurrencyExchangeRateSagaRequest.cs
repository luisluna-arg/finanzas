
using CQRSDispatch.Interfaces;
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Services.Requests.CurrencyExchangeRates;

public class DeleteCurrencyExchangeRateSagaRequest : ICommand, ISagaRequest
{
    public Guid Id { get; }

    public DeleteCurrencyExchangeRateSagaRequest(Guid id) : base()
    {
        Id = id;
    }
}
