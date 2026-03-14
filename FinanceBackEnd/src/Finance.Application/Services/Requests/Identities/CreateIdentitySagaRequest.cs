using Finance.Application.Commands;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Enums;

namespace Finance.Application.Services.Requests.Identities;

public class CreateIdentitySagaRequest : CreateIdentityCommand, ISagaRequest
{
    public CreateIdentitySagaRequest(Guid userId, IdentityProviderEnum provider, string sourceId)
        : base()
    {
        UserId = userId;
        Provider = provider;
        SourceId = sourceId;
    }
}
