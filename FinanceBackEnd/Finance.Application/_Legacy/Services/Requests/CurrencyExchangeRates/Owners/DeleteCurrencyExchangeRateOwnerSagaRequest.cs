using CQRSDispatch;
using Finance.Application.Legacy.Commands.Users;
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Commands.CurrencyExchangeRates.Owners;

public class DeleteCurrencyExchangeRateOwnerSagaRequest : OwnerBaseCommand<DataResult<bool>>, ISagaRequest
{
    public DeleteCurrencyExchangeRateOwnerSagaRequest(Guid id) : base()
    {
        Ids = [id];
    }

    public Guid[] Ids { get; set; } = [];
}
