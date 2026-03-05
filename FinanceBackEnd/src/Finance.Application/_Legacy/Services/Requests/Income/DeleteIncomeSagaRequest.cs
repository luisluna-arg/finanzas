
using CQRSDispatch.Interfaces;
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Services.Requests.Incomes;

public class DeleteIncomeSagaRequest : ICommand, ISagaRequest
{
    public DeleteIncomeSagaRequest(Guid id) : base()
    {
        Id = id;
    }

    public Guid Id { get; }
}
