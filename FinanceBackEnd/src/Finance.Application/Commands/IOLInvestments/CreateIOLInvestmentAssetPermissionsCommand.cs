using Finance.Domain.Models.Auth;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Commands.IOLInvestments;

public class CreateIOLInvestmentAssetPermissionsCommand
    : CreateResourcePermissionsCommand<IOLInvestmentAsset, Guid, IOLInvestmentAssetPermissions>;

public class CreateIOLInvestmentAssetPermissionsCommandHandler(FinanceDbContext dbContext)
    : CreateResourcePermissionsCommandHandler<CreateIOLInvestmentAssetPermissionsCommand, IOLInvestmentAsset, Guid, IOLInvestmentAssetPermissions>(dbContext)
{
    protected override async Task<IOLInvestmentAsset?> QuerySource(
        CreateIOLInvestmentAssetPermissionsCommand request, CancellationToken cancellationToken)
        => await DbContext.IOLInvestmentAsset
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == request.ResourceId, cancellationToken);
}
