using Finance.Application.Commands;
using Finance.Application.Services.Interfaces;

namespace Finance.Application.Services.Requests.Identities;

public sealed class DeleteIdentitySagaRequest : DeleteIdentityCommand, ISagaRequest
{
    public DeleteIdentitySagaRequest(Guid userId, Guid identityId)
        : base()
    {
        UserId = userId;
        IdentityId = identityId;
    }
}
