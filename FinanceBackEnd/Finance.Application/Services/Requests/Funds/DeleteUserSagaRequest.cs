
using CQRSDispatch.Interfaces;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Services.Requests.Funds;

public class DeleteFundSagaRequest : ICommand, ISagaRequest
{
    public Guid FundId { get; }

    public DeleteFundSagaRequest(Guid userId) : base()
    {
        FundId = userId;
    }
}
