
using CQRSDispatch.Interfaces;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Services.Requests.Incomes;

public class DeleteIncomeSagaRequest : ICommand, ISagaRequest
{
    public DeleteIncomeSagaRequest(Guid id) : base()
    {
        Id = id;
    }

    public Guid Id { get; }
}
