
using CQRSDispatch.Interfaces;
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Services.Requests.Funds;

public class DeleteFundSagaRequest : ICommand, ISagaRequest
{
    public DeleteFundSagaRequest(Guid id) : base()
    {
        Id = id;
    }

    public Guid Id { get; }
}
