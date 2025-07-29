
using CQRSDispatch.Interfaces;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Services.Requests.Funds;

public class DeleteFundSagaRequest : ICommand, ISagaRequest
{
    public DeleteFundSagaRequest(Guid id) : base()
    {
        Id = id;
    }

    public Guid Id { get; }
}
