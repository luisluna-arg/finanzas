using CQRSDispatch;
using Finance.Application.Commands.Users;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Commands.CurrencyExchangeRates.Owners;

public class DeleteCurrencyExchangeRateOwnerSagaRequest : OwnerBaseCommand<DataResult<bool>>, ISagaRequest
{
    public DeleteCurrencyExchangeRateOwnerSagaRequest(Guid id) : base()
    {
        Ids = [id];
    }

    public Guid[] Ids { get; set; } = [];
}
