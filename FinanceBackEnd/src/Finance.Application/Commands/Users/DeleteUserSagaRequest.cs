
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Commands.Users;

public sealed class DeleteUserSagaRequest : DeleteUserCommand, ISagaRequest
{
    public DeleteUserSagaRequest(Guid userId) : base()
    {
        UserId = userId;
    }
}
