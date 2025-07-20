
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Commands;

public class DeleteUserSagaRequest : DeleteUserCommand, ISagaRequest
{
    public DeleteUserSagaRequest(Guid userId) : base()
    {
        UserId = userId;
    }
}
