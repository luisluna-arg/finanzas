using CQRSDispatch;
using Finance.Application.Legacy.Commands.Users;
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Commands.CurrencyExchangeRates.Owners;

public abstract class BaseCurrencyExchangeRateOwnerSagaRequest<TResult> : OwnerBaseCommand<TResult>, ISagaRequest
    where TResult : RequestResult
{
    public BaseCurrencyExchangeRateOwnerSagaRequest(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
