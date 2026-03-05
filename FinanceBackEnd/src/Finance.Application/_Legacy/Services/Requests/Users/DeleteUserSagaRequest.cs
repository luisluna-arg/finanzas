
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Application.Legacy.Commands.Users;

public class DeleteUserSagaRequest : DeleteUserCommand, ISagaRequest
{
    public DeleteUserSagaRequest(Guid userId) : base()
    {
        UserId = userId;
    }
}
