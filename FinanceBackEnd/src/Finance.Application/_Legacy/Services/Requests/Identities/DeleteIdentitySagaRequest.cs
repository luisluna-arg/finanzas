using Finance.Application.Legacy.Commands;
using Finance.Application.Legacy.Services.Interfaces;

namespace Finance.Api.Controllers.Requests.Identities;

public sealed class DeleteIdentitySagaRequest : DeleteIdentityCommand, ISagaRequest
{
    public DeleteIdentitySagaRequest(Guid userId, Guid identityId)
        : base()
    {
        UserId = userId;
        IdentityId = identityId;
    }
}
