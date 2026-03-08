using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Debits;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.DebitOrigins;

public class CreateDebitOriginPermissionsCommand : CreateResourcePermissionsCommand<DebitOrigin, Guid, DebitOriginPermissions>;

public class CreateDebitOriginPermissionsCommandHandler(FinanceDbContext dbContext)
    : CreateResourcePermissionsCommandHandler<CreateDebitOriginPermissionsCommand, DebitOrigin, Guid, DebitOriginPermissions>(dbContext)
{
    protected override async Task<DebitOrigin?> QuerySource(
        CreateDebitOriginPermissionsCommand request, CancellationToken cancellationToken)
        => await DbContext.DebitOrigin
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.Id == request.ResourceId, cancellationToken);
}
