using CQRSDispatch;
using Finance.Application.Commands.Users;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Commands.CurrencyExchangeRates.Owners;

public abstract class BaseCurrencyExchangeRateOwnerSagaRequest<TResult> : OwnerBaseCommand<TResult>, ISagaRequest
    where TResult : RequestResult
{
    public BaseCurrencyExchangeRateOwnerSagaRequest(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
