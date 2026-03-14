using Finance.Application.Commands;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Enums;

namespace Finance.Application.Services.Requests.Identities;

public class UpdateIdentitySagaRequest : UpdateIdentityCommand, ISagaRequest
{
    public UpdateIdentitySagaRequest(Guid userId, Guid identityId, IdentityProviderEnum provider, string sourceId)
        : base()
    {
        UserId = userId;
        IdentityId = identityId;
        Provider = provider;
        SourceId = sourceId;
    }
}
