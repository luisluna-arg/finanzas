using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands;

public class CreateFundPermissionsCommand : CreateResourcePermissionsCommand<Fund, Guid, FundPermissions>;

public class CreateFundPermissionsCommandHandler(FinanceDbContext dbContext)
    : CreateResourcePermissionsCommandHandler<CreateFundPermissionsCommand, Fund, Guid, FundPermissions>(dbContext)
{
    protected override async Task<Fund?> QuerySource(CreateFundPermissionsCommand request, CancellationToken cancellationToken)
    {
        return await DbContext.Fund
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(f => f.Id == request.ResourceId, cancellationToken);
    }
}
